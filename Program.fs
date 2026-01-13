open Microsoft.UI
open Microsoft.Windows.ApplicationModel.DynamicDependency

type App(content: unit -> Xaml.UIElement) =
    inherit Xaml.Application()
    
    override a.OnLaunched(args: Xaml.LaunchActivatedEventArgs) =        
        let window = Xaml.Window(
            Content = content()
        )
        window.Activate()

        ()

/// <param name="content">The content of the window as a function to avoid constructing views before WinUI 3 is initialised</param>
let run (content: unit -> Xaml.UIElement) =
    // Windows App SDK version 1.8
    // Not sure how to get this without hardcoding it
    let majorMinorVersion = 0x00010008u

    let success, _ = Bootstrap.TryInitialize majorMinorVersion

    if not success then
        exit 1

    WinRT.ComWrappersSupport.InitializeComWrappers()
    Xaml.Application.Start(fun _ ->
        let context = Dispatching.DispatcherQueueSynchronizationContext(
            Dispatching.DispatcherQueue.GetForCurrentThread()
        )
        System.Threading.SynchronizationContext.SetSynchronizationContext context
        App content |> ignore
    )

    Bootstrap.Shutdown()
    0

let stackPanel (children: Xaml.UIElement list) =
    let sp = Xaml.Controls.StackPanel()
    children |> List.iter sp.Children.Add
    sp :> Xaml.UIElement

let textBlock (txt: string) =
    Xaml.Controls.TextBlock(Text = txt)

// WinUI 3 requires single-threaded COM
// https://learn.microsoft.com/en-us/dotnet/api/system.stathreadattribute?view=net-10.0
[<EntryPoint; System.STAThread>]
let main argv =
    run (fun () -> stackPanel [
            textBlock "WinUI from F#!"
        ]
    )

