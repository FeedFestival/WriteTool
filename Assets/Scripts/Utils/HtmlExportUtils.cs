
using System.Collections.Generic;

namespace Assets.Scripts.Utils
{
    public static class HtmlExportUtils
    {
        public static string GetBaseStart()
        {
            return @"<!DOCTYPE html>
                <html lang=""en/"">
                <head>
                    <meta charset=""utf-8"">
                    <title>title</title>
                    " + Css() + @"
                    <script src=""script.js""></script>
                </head>
                <body>";
        }

        public static string GetBaseEnd()
        {
            return @"</body>
            </html>";
        }

        public static string PageStart()
        {
            return @"<div class=""page"">";
        }

        public static string Element(Element element)
        {
            return @"<div class=""element " + GetElementClass(element.ElementType) + @""" id=""" + element.Id + @""">
                " + element.Text + @"
            </div>";
        }

        public static string Picture(Element element, List<string> fileNames)
        {
            string elementHtml = @"<div class=""element picture " + (fileNames.Count > 1 ? "many" : "") + @""" id=""" + element.Id + @""">";
            foreach (var fileName in fileNames)
            {
                elementHtml += @"<img src=""" + fileName + @""">";
            }
            elementHtml += "</div>";
            return elementHtml;
        }

        private static string GetElementClass(ElementType elementType)
        {
            switch (elementType)
            {
                case ElementType.SceneHeading:
                    return "scene-heading";
                case ElementType.Action:
                    return "action";
                case ElementType.Character:
                    return "character";
                case ElementType.Dialog:
                    return "dialog";
                default:
                    return "picture";
            }
        }

        public static string DivTagEnd()
        {
            return "</div>";
        }

        private static string Css()
        {
            return @"<style>
                body {
                    width: 100vw;
                    min-height: 100vh;
                    margin: 0 !important;
                    padding: 0 !important;
                }

                * {
                    box-sizing: border-box;
                    -moz-box-sizing: border-box;
                }

                .page {
                    width: 210mm;
                    /* min-height: 297mm; */
                    padding: 20mm;
                    margin: 10mm auto;
                    border: 1px #D3D3D3 solid;
                    border-radius: 5px;
                    background: white;
                    box-shadow: 0 0 5px rgba(0, 0, 0, 0.1);
                }

                .subpage {
                    padding: 1cm;
                    border: 5px red solid;
                    height: 257mm;
                    outline: 2cm #FFEAEA solid;
                }

                @page {
                    size: A4;
                    margin: 0;
                }

                @media print {

                    html,
                    body {
                        width: 210mm;
                        height: 297mm;
                    }

                    .page {
                        margin: 0;
                        border: initial;
                        border-radius: initial;
                        width: initial;
                        min-height: initial;
                        box-shadow: initial;
                        background: initial;
                        page-break-after: always;
                    }
                }

                .element {
                    display: block;
                }

                .element.scene-heading {
                    background-color: lightgray;
                    padding: 0 1mm;
                }

                .element.action {
                    padding: 5mm 20mm;
                }

                .element.character {
                    padding: 1mm 50mm;
                    text-align: center;
                    text-transform: uppercase;
                    font-weight: 700;
                }

                .element.dialog {
                    padding: 1mm 50mm;
                    text-align: center;
                    text-transform: capitalize;
                    font-size: 4mm;
                }

                .element.picture {
                    padding: 5mm 20mm;
                    min-height: 70mm;
                    width: 170mm;
                }

                .element.picture img {
                    width: 100%;
                    height: auto !important;
                    /* background: url(http://mehararts.com/category_image/demo.png); */
                }

                .picture.many {
                    padding: 5mm 5mm !important;
                    display: inline-block !important;
                }

                .picture.many img {
                    width: 48% !important;
                    padding: 0 1% !important;
                }
            </style>
            ";
        }
    }
}
