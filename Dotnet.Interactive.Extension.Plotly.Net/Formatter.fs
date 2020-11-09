namespace Dotnet.Interactive.Extension.Plotly.Net

open System
open System.IO
open System.Threading.Tasks
open Microsoft.DotNet.Interactive
open Microsoft.DotNet.Interactive.Formatting
open Plotly.NET
open Plotly.NET.GenericChart

type PlotlyNetFormatterKernelExtension() =

    let registerFormatter () =
        Formatter.Register<GenericChart.GenericChart>
            (Func<FormatContext, GenericChart.GenericChart, TextWriter, bool>(fun context chart writer ->
                let html = toChartHTML chart
                writer.Write(html)

                true),
             HtmlFormatter.MimeType)

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
