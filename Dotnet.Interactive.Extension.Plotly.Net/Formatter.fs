namespace Dotnet.Interactive.Extension.Plotly.Net

open System.Threading.Tasks
open Microsoft.DotNet.Interactive
open Microsoft.DotNet.Interactive.Formatting
open Plotly.NET.GenericChart

type PlotlyNetFormatterKernelExtension() =

    let registerFormatter () =
        Formatter.Register<GenericChart>((fun chart writer ->
            let html = toChartHTML chart
            let hackedHtml =
                html.Replace(
                    """var fsharpPlotlyRequire = requirejs.config({context:'fsharp-plotly',paths:{plotly:'https://cdn.plot.ly/plotly-latest.min'}})""",
                    """var fsharpPlotlyRequire = requirejs.config({context:'fsharp-plotly',paths:{plotly:'https://cdn.plot.ly/plotly-latest.min'}}) || require;"""
                )
            writer.Write(hackedHtml)), HtmlFormatter.MimeType)

    interface IKernelExtension with
        member _.OnLoadAsync _ =
            registerFormatter ()

            if isNull KernelInvocationContext.Current |> not then
                let message =
                    (nameof PlotlyNetFormatterKernelExtension, nameof GenericChart)
                    ||> sprintf "Added %s including formatters for %s"

                KernelInvocationContext.Current.Display(message, "text/markdown")
                |> ignore

            Task.CompletedTask
