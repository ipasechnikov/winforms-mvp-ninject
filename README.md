# WinForms app with Model-View-Presenter pattern and Ninject DI

This is a small example project that shows how you can utilize MVP (Model-View-Presenter) pattern and Ninject dependency injector in your good old WinForms application.

I hope this example will come in handy for other developers who want to improve code quality of their WinForms application.

## Foreword

Recently I've been working quite a lot on legacy WinForms applications.
Refactoring and maintaning these applications is a crucial process.
After spending some time with various codebases, I decided to incorporate better practices like MVP pattern and Dependency Injection.

I've also tried MVVM (Model-View-ViewModel) pattern with WinForms but it didn't work out well. 
It works but it looks alien and out of place in case of WinForms application.

## Getting Started

In this section I'll try to explain how everything is tied together.

### Prerequisites

#### .NET Core or .NET Framework 4.8

I use .NET Core 6 for this project but you are free to use any version of .NET Core.
What's is more, actually all of my work projects utilize .NET Framework 4.8.
So you can even use .NET Framework. In case of .NET Framework I recommend using the latest (last) available version which is .NET Framework 4.8.1.

#### Visual Studio

I use Visual Studio 2022 Community Edition. If you have an older version of Visual Studio then there might be problems with openning solution.
But nevertheless the techniques used in the solution should be totally applicable for older versions of Visual Studio.

#### Why Ninject?

There is quite a few great dependency injector projects available for C# but why did I choose Ninject?
It's simple, it's the only dependency injector I've ever used since a very long time ago and I find it quite easy to use. I'm not proud of these words myself.

I beleive you can use any other dependency injection solution such as Autofac. If you know one dependency injector, you know pretty much all of them.
The problem is to make dependency injector work with WinForms.

### Project structure

There are 2 projects in the solution

* **WinFormsMvpNinject.App** - WinForms app
* **WinFormsMvpNinject.Tests** - Tests for WinForms App

#### WinFormsMvpNinject.App

We'll start with WinFormsMvpNinject.App at first. Its structure may look a bit complicated at first glance for an example project but it's only because I tried to make example as close to actual production application as possible.

##### "Ninject/Modules" folder

Here you place all you dependncy injection configurations. If you are reading this README then I think there is no need to explain it. But if for some reason you are a newcommer then I recommend you reading a few pages of an awesome [Ninject Wiki](https://github.com/ninject/Ninject/wiki).

##### "Ninject/Strategies" folder

This is a "magic" folder that allows Ninject to work with WinForms. Basically you just copy-paste this folder into your project, configure your `Main` method to use classes from these folder and forget about it.

```csharp
internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        // Create a Ninject kernel
        var kernel = new StandardKernel(new MainModule());

        // This one line makes DI magically work with WinForms
        // It allows us to recursively inject controls inside forms or other controls
        kernel.Components.Add<IActivationStrategy, WindowsFormsStrategy>();

        // Get MainForm with injected dependencies and run application
        var mainForm = kernel.Get<MainForm>();
        Application.Run(mainForm);
    }
}
```

#### "Models" folder

Nothing special. We place Models of MVP pattern here. In our case we have a tiny `IUser` interface and its implementation `DefaultUser`.

#### "Views" folder

Here we place our Views of MVP pattern. Let's take a closer look at code of `IMainView` interface and its implementation `MainForm`.

First of all, we have to inject presenter into our form. 
We use property injection because WinForms use a style when components have default constructor and everything else is initialized via properties after the object is created.

View instance is the one that initialized presenter by setting its View property to reference the View.

```csharp
private IMainPresenter? presenter;

[Inject]
public IMainPresenter? Presenter
{
    get => presenter;
    set
    {
        // View can have only single presenter associated with it
        if (presenter != null)
            presenter.View = null;

        if (value == null)
            throw new ArgumentNullException(nameof(value));

        // Set and initialize a new presenter
        presenter = value;
        presenter.View = this;
    }
}
```

Because of cyclic reference between View and Presenter we resolve it during View disposure by runtime otherwise garbage collector won't be able to collect disposed view and its presenter. That's why we add a handler to `Disposed` event to remove cyclic reference by nulling Presenter's `View` property.

```csharp
public MainForm()
{
    InitializeComponent();
    Disposed += (sender, args) =>
    {
        // Resolve cyclic reference to let GC collect the objects
        if (presenter != null)
        {
            presenter.View = null;
            presenter = null;
        }
    };
}
```

#### "Presenters" folder

This folder contains Presenters of MVP pattern. Compared to Views, Presenters don't have much going on in terms of injections and cleaning up. 

Presenter should have a reference to its View so that it can interact with it by setting View properties.

```csharp
public IMainView? View
{
    get; set;
}

public async Task GetUsers()
{
    View!.Users = await userService.GetUsers();
}
```

#### "Services" folder

This one contains your services that you may use in Presenters to get users from API, database or something else.
In our case we have a dummy service that always returns hardcoded users.

```csharp
public class DefaultUserService : IUserService
{
    public Task<IUser[]> GetUsers()
    {
        return Task.FromResult(new IUser[]
        {
            new DefaultUser { Name = "Name1", Age = 10 },
            new DefaultUser { Name = "Name2", Age = 20 },
            new DefaultUser { Name = "Name3", Age = 30 },
            new DefaultUser { Name = "Name4", Age = 40 },
            new DefaultUser { Name = "Name5", Age = 50 }
        });
    }
}
```

#### "Factories" folder

We are not interested in this folder itself but in one interface that in contains.
`IViewFactory` is an interface that is bound by Ninject in `MainModule` in a special way.

Interface itself doesn't have much.

```csharp
// Allows to create us injected forms and control dynamically at runtime
public interface IViewFactory
{
    ISomeRandomView CreateSomeRandomView();
}
```

The part we are interested in the most is binding in `MainModule`. This binding make it possible to create injected forms and controls at runtime dynamically.
It may not sound that big, but if you try to create a form that has injected dependencies at runtime you'll run into a few issues. One of which is how do you do it? How do you even inject its dependencies?

Probably it would look like something like this. You'll create or pass all dependencies manually. These were only a few dependencies and there can be much much more of them in a real application. The code will be hard to maintain and it will look ugly.

```csharp
private void btnOpenSomeRandomView_Click(object sender, EventArgs e)
{
    var form = new SomeRandomForm();
    var presenter = new DefaultSomeRandomPresenter();
    presenter.Service = new DefaultUserService();
    form.Presenter = presenter;
    form.Show();
}
```

`IViewFactory` allows us to inject all dependencies automatically. Ninject injects them and we have a very clean code.

```csharp
[Inject]
public IViewFactory? ViewFactory
{
    get; set;
}

private void btnOpenSomeRandomView_Click(object sender, EventArgs e)
{
    // This is how you create an injected form or control during runtime. Amazing!
    var form = (Form)ViewFactory!.CreateSomeRandomView();
    form.Show();
}
```

`MainModule` binds `IViewFactory` as a Ninject factory that makes all the magic happen.

```csharp
public class MainModule : NinjectModule
{
    public override void Load()
    {
        ...
        BindFactories();
    }
    
    ...

    private void BindFactories()
    {
        // A neat Ninject extension that in our case allows us to create injected forms/control dynamically
        // https://github.com/ninject/Ninject.Extensions.Factory/wiki
        Bind<IViewFactory>().ToFactory();
    }
}
```

#### WinFormsMvpNinject.Tests

This project contains a single test class `MemoryLeaksTest` that checks if cyclic reference issue previously explained is resolved correctly.

```csharp
[Test]
public void MainForm_Presenter_IsNull_OnDispose()
{
    var mainForm = kernel.Get<MainForm>();

    // Make sure that presenter was curretly injected
    Assert.IsNotNull(mainForm.Presenter);

    // Dispose form as if it was closed by the user or something
    mainForm.Dispose();

    // Make sure that presenter was curretly set to null by Disposed event handler
    // This means that mainForm will be collected by GC because cyclic references are resolved
    Assert.IsNull(mainForm.Presenter);
}
```

## Built With

* [Ninject](https://github.com/ninject/Ninject) - Dependency Injector
* [Ninject.Extensions.Factory](https://github.com/ninject/Ninject.Extensions.Factory) - Ninject extension that allows us to create injected WinForms forms and controls dynamically

## Contributing

I would be really pleased if someone could help me to resolve boilerplate issue with setting and cleaning Presenter.

These two boilerplate pieces will be all over your codebase in every View. It's not that big of a problem but it would be nice not to have them.

```csharp
[Inject]
public IMainPresenter? Presenter
{
    get => presenter;
    set
    {
        // View can have only single presenter associated with it
        if (presenter != null)
            presenter.View = null;

        if (value == null)
            throw new ArgumentNullException(nameof(value));

        // Set and initialize a new presenter
        presenter = value;
        presenter.View = this;
    }
}
```

```csharp
public MainForm()
{
    InitializeComponent();
    Disposed += (sender, args) =>
    {
        // Resolve cyclic reference to let GC collect the objects
        if (presenter != null)
        {
            presenter.View = null;
            presenter = null;
        }
    };
}
```

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* Used [this nice GitHub repo](https://github.com/mrts/winforms-mvp) to implement MVP pattern in this pattern
* [This StackOverflow answer](https://stackoverflow.com/a/33928388) allows to use Ninject with WinForms
