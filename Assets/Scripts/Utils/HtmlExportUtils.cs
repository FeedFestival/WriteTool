
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Assets.Scripts.Utils
{
    public static class HtmlExportUtils
    {
        private static Regex newLineRegex = new Regex(@"(\r\n|\r|\n)+");

        public static string GetBaseStart()
        {
            return @"<!DOCTYPE html>
                <html lang=""en/"">
                <head>
                    <meta charset=""utf-8"">
                    <title>title</title>
                    " + Css() + @"
                    <script src=""https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js""></script>
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
            string newText = newLineRegex.Replace(element.Text, "<br /> <br />");
            return @"<div class=""element " + GetElementClass(element.ElementType) + @""" id=""" + element.Id + @""">
                " + newText + @"
            </div>";
        }

        public static string Picture(Element element)
        {
            string elementHtml = @"<div class=""element picture " + (element.FileNames.Count > 1 ? "many" : "") + @""" id=""" + element.Id + @""">";
            foreach (var fileName in element.FileNames)
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

                @font-face {
                    src: url(../GraphikRegular.otf);
                    font-family: graphikRegular;
                }
                @font-face {
                    src: url('../robotoslab-regular.woff2') format('woff2'),
                        url('../robotoslab-regular.woff') format('woff');
                    font-weight: normal;
                    font-style: normal;
                    font-family: robotoSlabRegular;
                }

                .element.action,
                .element.dialog {
                    font-family: graphikRegular;
                    font-size: 15px;
                }
                .element.scene-heading,
                .element.character {
                    font-family: robotoSlabRegular;
                    font-size: 14px;
                }

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
                    text-align: center;
                }

                .element.picture img {
                    max-height: 250px;
                    /* background: url(http://mehararts.com/category_image/demo.png); */
                }

                .picture.many {
                    padding: 5mm 5mm !important;
                    display: inline-block !important;
                }

                .picture.many img {
                    // width: 48% !important;
                    padding: 0 1% !important;
                }

                .page-nr {
                    text-align: center; 
                    height: 7px; 
                    font-size: 10px;
                    position: relative; 
                    bottom: 0; 
                    display: block;
                }
            </style>
            ";
        }

        public static string PageScript()
        {
            return @"
            <script>
                setTimeout(() => {

                    $('.page').attr('id', 'old');
                    var body = $('body');

                    // add new page
                    var pageNr = 1;
                    var pageElement = createPage(body, pageNr);

                    // get elements
                    var elements = $('.element');

                    // determine page settings
                    const pageDescrSize = 7;
                    // const maxMm = 297;
                    const maxMm = ((297 - 40) - 7);
                    let currentPageMm = 0;
                    let addPreviousElement = false;

                    for (let i = 0; i < elements.length; i++) {
                        const element = $(elements[i]);
                        const previousElement = $(elements[i - 1]);

                        const widthMm = convertToMm(element.height()) + convertToMm(element.css('padding-top'), true) + convertToMm(element.css('padding-bottom'), true);
                        const wouldBeMm = (currentPageMm + widthMm);

                        if (addPreviousElement) {
                            addPreviousElement = false;
                        } else if (wouldBeMm > maxMm) {

                            addPageNr(pageElement, pageNr, maxMm - currentPageMm);

                            // add new page
                            currentPageMm = 0;
                            pageNr++;
                            pageElement = createPage(body, pageNr);

                            // start with scene heading if possible
                            if (previousElement.hasClass('scene-heading') || previousElement.hasClass('character')) {
                                i -= 2;
                                addPreviousElement = true;
                                continue;
                            }
                        }

                        currentPageMm += widthMm;
                        pageElement.append(element);

                        if (i === elements.length - 1) {
                            addPageNr(pageElement, pageNr, maxMm - currentPageMm);
                        }
                    }
                    $('#old').remove();
                }, 10);

                function addPageNr(pageElement, pageNr, margin) {
                    pageElement.append(
                        '<div class=""page-nr"" style=""margin-top: ' + (margin <= 0 ? 0 : margin) + 'mm;"">'
                            + pageNr +
                        '</div>');
                }

                function createPage(body, pageNr) {
                    body.append('<div class=""page"" id=""page_' + pageNr + '""></div>');
                    var pageElement = $('#page_' + pageNr);
                    pageElement.css('min-height', '297mm');
                    return pageElement;
                }

                function convertToMm(pixels, asPx) {
                    if (asPx) {
                        pixels = parseFloat(pixels.replace('px', ''));
                    }
                    // 1 px = 0.264583333;
                    return pixels * 0.264583333;
                }
            </script>
            ";
        }
    }
}
