using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.CodeRush.Core;
using DevExpress.CodeRush.PlugInCore;
using DevExpress.CodeRush.StructuralParser;
using System.Collections;

namespace CR_ReverseOperands
{
    public partial class PlugIn1 : StandardPlugIn
    {
        // DXCore-generated code...
        #region InitializePlugIn
        public override void InitializePlugIn()
        {
            base.InitializePlugIn();
            registerReverseOperands();
        }
        #endregion
        #region FinalizePlugIn
        public override void FinalizePlugIn()
        {
            base.FinalizePlugIn();
        }
        #endregion
        public void registerReverseOperands()
        {
            DevExpress.CodeRush.Core.CodeProvider ReverseOperands = new DevExpress.CodeRush.Core.CodeProvider(components);
            ((System.ComponentModel.ISupportInitialize)(ReverseOperands)).BeginInit();
            ReverseOperands.ProviderName = "ReverseOperands"; // Should be Unique
            ReverseOperands.DisplayName = "Reverse Operands";
            ReverseOperands.CheckAvailability += ReverseOperands_CheckAvailability;
            ReverseOperands.Apply += ReverseOperands_Apply;
            ((System.ComponentModel.ISupportInitialize)(ReverseOperands)).EndInit();
        }

        private LanguageElement _startElement;
        private LanguageElement _endElement;
        private void ReverseOperands_CheckAvailability(Object sender, CheckContentAvailabilityEventArgs ea)
        {
            TextDocument ActiveDoc = ea.TextDocument;

            SourceRange Range;
            ea.TextDocument.GetSelection(out Range);
            if (!Range.StartPrecedesEnd)
                Range = new SourceRange(Range.End, Range.Start);
            SourcePoint StartPoint = Range.Start;
            SourcePoint EndPoint = Range.End;

            _startElement = ActiveDoc.GetNodeAt(StartPoint);
            _endElement = ActiveDoc.GetNodeAt(EndPoint);

            // Exit if selectionText does not contain ','
            if (_startElement == _endElement)
                return;

            if (_startElement.Parent == null)
                return; // not sure how, but cover ourselves anyway :)

            BinaryOperatorExpression  Operator = GetSharedBinaryOperator(CodeRush.Documents.ActiveTextDocument, _startElement, _endElement);
            if (Operator == null)
                return;
            _startElement = Operator.LeftSide;
            _endElement = Operator.RightSide;
            ea.Available = true;
        }

        private BinaryOperatorExpression GetSharedBinaryOperator(TextDocument doc, LanguageElement startElement, LanguageElement endElement)
        {

            // return first common factor which is also BinaryOperatorExpression
            IEnumerable<LanguageElement> AllStartAncestors = getAllAncestors(startElement);
            IEnumerable<LanguageElement> AllEndAncestors = getAllAncestors(endElement);

            // Not sure of this ??? Test 
            var FirstCommon = AllStartAncestors.Intersect(AllEndAncestors)
                              .First(f => f.ElementType.In(LanguageElementType.BinaryOperatorExpression,
                                                           LanguageElementType.LogicalOperation, 
                                                           LanguageElementType.RelationalOperation));
            return ((BinaryOperatorExpression)FirstCommon);
        }
        private IEnumerable<LanguageElement> getAllAncestors(LanguageElement element)
        {
            List<LanguageElement> results = new List<LanguageElement>();
            results.Add(element);
            while (element.Parent != null)
            {
                element = element.Parent;
                results.Add(element);
            }
            return results;
        }
        private static List<LanguageElement> GetNodesInRange(TextDocument Doc, SourceRange range)
        {
            var Nodes = new List<LanguageElement>();
            for (int i = range.Start.Offset; i < range.End.Offset; i++)
            {

                LanguageElement Node = Doc.GetNodeAt(new SourcePoint(range.Start.Line, i));
                if (!Nodes.Contains(Node))
                    Nodes.Add(Node);
            }
            return Nodes;
        }
        private void ReverseOperands_Apply(Object sender, ApplyContentEventArgs ea)
        {
            SourceRange Range;

            using (ea.TextDocument.NewCompoundAction("Reverse Operands"))
            {
                ea.TextDocument.GetSelection(out Range);
                ReverseLanguageElements(_startElement, _endElement);
                CodeRush.Selection.SelectRange(Range);
               
            }
            
        }
        private void ReverseLanguageElements(LanguageElement Element1, LanguageElement Element2)
        {
            SourceRange Range1 = Element1.Range;
            SourceRange Range2 = Element2.Range;

            string Code1 = CodeRush.Language.GenerateElement(Element1);
            string Code2 = CodeRush.Language.GenerateElement(Element2);

            TextDocument ActiveDoc = CodeRush.Documents.ActiveTextDocument;
            ActiveDoc.QueueReplace(Element1, Code2);
            ActiveDoc.QueueReplace(Element2, Code1);
            ActiveDoc.ApplyQueuedEdits();
        }
    }
    public static class LanguageElementExt
    {
        public static bool In(this LanguageElementType source, params LanguageElementType[] types)
        {
            return types.Contains(source);
        }
    }
}