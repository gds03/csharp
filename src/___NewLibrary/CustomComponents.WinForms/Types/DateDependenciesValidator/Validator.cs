//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CustomComponents.WinForms.Types.DateDependenciesValidator
//{
//    /// <summary>
//    ///     Class that sets metadata for controls, and validates all dependencies between those controls
//    /// </summary>
//    /// <typeparam name="TValue"></typeparam>
//    public class Validator<TValue>
//        where TValue : struct, IComparable<TValue>
//    {

//        /// <summary>
//        ///     Struct that is mantained internally by each node in the list
//        /// </summary>
//        class InternalNode
//        {
//            // This editor
//            internal UltraDateTimeEditor m_currentEditor;

//            // Depends of this editor
//            internal UltraDateTimeEditor m_parentEditor;

//            // With this signal
//            internal Signal m_signal;

//            // And if this field, is:
//            //      true: Indicates that is required and Value must be != NULL. If is NULL, the editor will be added on NullableList
//            //      false: Indicates that this field is not required (validation passes OK if m_currentEditor.Value is null)
//            internal bool m_currentIsRequired;
//        }



//        // List that mantains the dependencies
//        readonly LinkedList<InternalNode> m_list = new LinkedList<InternalNode>();

//        // Indicates that if established the first control that is not dependent.
//        bool m_headEstablished;




//        /// <summary>
//        ///     Indicates that Current control is dependent of the Parent control and if is or not required.
//        ///     If required, when validating, if the Value is NULL, is added to NullableList.
//        ///     Typically this method is called when establishing the head (we don't have any signal to set)
//        /// </summary>
//        public void AddDependency(UltraDateTimeEditor Current, UltraDateTimeEditor Parent, bool IsCurrentRequired)
//        {
//            AddDependency(Current, Parent, IsCurrentRequired, Signal.Equal);
//        }


//        /// <summary>
//        ///     Indicates that Current control is dependent of the Parent control, if is or not required and what must be the signal to compare.
//        ///     If required, when validating, if the Value is NULL, is added to NullableList.
//        /// </summary>
//        public void AddDependency(UltraDateTimeEditor Current, UltraDateTimeEditor Parent, bool IsCurrentRequired, Signal Signal)
//        {
//            if (!m_headEstablished)
//            {
//                if (Parent != null)
//                    throw new InvalidOperationException("The first dependency must be null");

//                else m_headEstablished = true;
//            }

//            else
//            {
//                if (Parent == null)
//                    throw new InvalidOperationException("Only one dependency can be null");
//            }

//            //
//            // Invariats checking

//            if (Current == null)
//                throw new InvalidOperationException("Current cannot be null");


//            // Create node and add to the list
//            InternalNode node = new InternalNode
//            {
//                m_currentEditor = Current,
//                m_parentEditor = Parent,

//                m_currentIsRequired = IsCurrentRequired,
//                m_signal = Signal
//            };

//            m_list.AddLast(node);
//        }




//        /// <summary>
//        ///     Validate all controls with each specific rules.
//        /// </summary>
//        /// <returns>false if some controls are indicated required and are null or if some controls don't have the rules correct. Otherwise return true</returns>
//        public bool ValidateAll(out IEnumerable<DependencyResult> InvalidInfo, out IEnumerable<UltraDateTimeEditor> NullableControls)
//        {
//            return Validate(null, out InvalidInfo, out NullableControls);
//        }


//        /// <summary>
//        ///     Validate all controls until Editor with each specific rules.
//        /// </summary>
//        /// <returns>false if some controls are indicated required and are null or if some controls don't have the rules correct. Otherwise return true</returns>
//        public bool ValidateUntil(UltraDateTimeEditor Editor, out IEnumerable<DependencyResult> InvalidInfo, out IEnumerable<UltraDateTimeEditor> NullableControls)
//        {
//            return Validate(Editor, out InvalidInfo, out NullableControls);
//        }







//        #region Internal Methods

//        bool Validate(UltraDateTimeEditor Editor, out IEnumerable<DependencyResult> InvalidInfo, out IEnumerable<UltraDateTimeEditor> NullableControls)
//        {
//            if (!m_headEstablished)
//                throw new InvalidOperationException("First you must configure the header");

//            if (m_list.Count <= 1)
//                throw new InvalidOperationException("You should have at least 2 elements to validate");

//            List<DependencyResult> i = new List<DependencyResult>();
//            List<UltraDateTimeEditor> n = new List<UltraDateTimeEditor>();
//            InvalidInfo = i;
//            NullableControls = n;

//            LinkedListNode<InternalNode> currNode = m_list.First.Next;                                  // Ignore 1st node (nothing to verify)
//            int DeterminatedCount = (Editor != null) ? GetCountForEditor(Editor) : m_list.Count;      // If editor is passed, we just validate until that validator

//            // Iterate through each control
//            for (int idx = 1; idx < DeterminatedCount; idx++, currNode = currNode.Next)
//            {
//                InternalNode currNodeValue = currNode.Value;

//                // 
//                // Verifications

//                object CurrentValue = currNodeValue.m_currentEditor.Value;

//                if (CurrentValue == null)
//                {
//                    if (currNodeValue.m_currentIsRequired)
//                    {
//                        //
//                        // Is required and Current Value is NULL, so add to the list of nullables
//                        // to indicate to the user that this control have a NULL value and is required.

//                        n.Add(currNodeValue.m_currentEditor);
//                    }
//                }

//                else
//                {
//                    //
//                    // CurrentValue != null

//                    LinkedListNode<InternalNode> ParentNode = SearchParentValueNullInChain(currNode);

//                    //
//                    // Validate only if the ParentNode have the value

//                    if (ParentNode.Value.m_currentEditor.Value != null)
//                        CompareCurrentWithDependent(i, currNodeValue, ParentNode.Value);
//                }
//            }

//            return i.Count == 0 && n.Count == 0;
//        }


//        /// <summary>
//        ///     Get the index within the list where Editor is setted.
//        /// </summary>
//        int GetCountForEditor(UltraDateTimeEditor Editor)
//        {
//            LinkedListNode<InternalNode> currNode = m_list.First.Next;

//            for (int idx = 0; idx < m_list.Count; idx++, currNode = currNode.Next)
//            {
//                if (currNode.Value.m_currentEditor == Editor)
//                    return ++idx;   // ++Idx because we want to return that idx (inclusive) to the for loop
//            }

//            // If not found, return the m_list count
//            return m_list.Count;
//        }





//        /// <summary>
//        ///     Searches for the parent in the chain that contain a null value.
//        /// </summary>
//        LinkedListNode<InternalNode> SearchParentValueNullInChain(LinkedListNode<InternalNode> fromNode)
//        {
//            // Go until find some node.value != null
//            for (fromNode = fromNode.Previous; (fromNode != m_list.First && fromNode.Value.m_currentEditor.Value == null); fromNode = fromNode.Previous) ;
//            return fromNode;
//        }










//        /// <summary>
//        ///     Adds to l a DependencyResult object the current editor that depends of another editor with a specific signal.
//        /// </summary>
//        static void AddToList(List<DependencyResult> l, Signal expectedSignal, UltraDateTimeEditor current, UltraDateTimeEditor dependsOn)
//        {
//            l.Add(new DependencyResult
//            {
//                ExpectedSignal = expectedSignal,
//                Editor = current,
//                EditorDependsOn = dependsOn
//            });
//        }


//        /// <summary>
//        ///     Adds to the l list a DependencyResult object if the Rule (or signal) is not verifier between the Parent and the Current Editor.
//        /// </summary>
//        static void CompareCurrentWithDependent(List<DependencyResult> l, InternalNode currNode, InternalNode parentNode)
//        {
//            TValue CurrentValue = (TValue)currNode.m_currentEditor.Value;
//            TValue ParentValue = (TValue)parentNode.m_currentEditor.Value;
//            Signal signal = currNode.m_signal;

//            switch (signal)
//            {
//                // ==
//                case Signal.Equal:
//                    if (CurrentValue.CompareTo(ParentValue) != 0)
//                        AddToList(l, Signal.Equal, currNode.m_currentEditor, parentNode.m_currentEditor);

//                    break;

//                // !=
//                case Signal.NotEqual:
//                    if (CurrentValue.CompareTo(ParentValue) == 0)
//                        AddToList(l, Signal.NotEqual, currNode.m_currentEditor, parentNode.m_currentEditor);

//                    break;

//                // <
//                case Signal.Lesser:
//                    if (CurrentValue.CompareTo(ParentValue) >= 0)
//                        AddToList(l, Signal.Lesser, currNode.m_currentEditor, parentNode.m_currentEditor);

//                    break;

//                // >
//                case Signal.Greater:
//                    if (CurrentValue.CompareTo(ParentValue) <= 0)
//                        AddToList(l, Signal.Greater, currNode.m_currentEditor, parentNode.m_currentEditor);

//                    break;

//                // <=
//                case Signal.LesserOrEqual:
//                    if (CurrentValue.CompareTo(ParentValue) > 0)
//                        AddToList(l, Signal.LesserOrEqual, currNode.m_currentEditor, parentNode.m_currentEditor);

//                    break;

//                // >=
//                case Signal.GreaterOrEqual:
//                    if (CurrentValue.CompareTo(ParentValue) < 0)
//                        AddToList(l, Signal.GreaterOrEqual, currNode.m_currentEditor, parentNode.m_currentEditor);

//                    break;

//                default: throw new InvalidOperationException("Invalid Signal State!!!");
//            }
//        }






//        #endregion
//    }
//}
