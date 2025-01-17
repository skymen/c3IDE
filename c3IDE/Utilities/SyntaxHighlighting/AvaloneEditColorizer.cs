﻿using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace c3IDE.Utilities.SyntaxHighlighting
{
    public class AvaloneEditColorizer : DocumentColorizingTransformer
    {
        //todo: experiment with avalon edits document colorizer
        //usage: EditTimePluginTextEditor.TextArea.TextView.LineTransformers.Add(new AvaloneEditColorizer());
        protected override void ColorizeLine(DocumentLine line)
        {
            int lineStartOffset = line.Offset;
            string text = CurrentContext.Document.GetText(line);
            int start = 0;
            int index;
            while ((index = text.IndexOf("AvalonEdit", start)) >= 0)
            {
                base.ChangeLinePart(
                    lineStartOffset + index, // startOffset
                    lineStartOffset + index + 10, // endOffset
                    (VisualLineElement element) =>
                    {
                        // This lambda gets called once for every VisualLineElement
                        // between the specified offsets.
                        Typeface tf = element.TextRunProperties.Typeface;
                        // Replace the typeface with a modified version of
                        // the same typeface
                        element.TextRunProperties.SetTypeface(new Typeface(
                            tf.FontFamily,
                            FontStyles.Italic,
                            FontWeights.Bold,
                            tf.Stretch
                        ));
                    });
                start = index + 1; // search for next occurrence
            }
        }
    }
}
