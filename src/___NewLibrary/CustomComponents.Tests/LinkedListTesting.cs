using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CustomComponents.Algorithms.Collections.Generic;

namespace CustomComponents.Tests
{
    [TestClass]
    public class LinkedListTesting
    {
        int[] values = new int[] { 1, 2, 3, 4, 4, 5, 10, 20, 50, 100, 200, 1000 };
        LinkedList<int> list;


        [Description("Create a list for each test use it.")]
        [TestInitialize]
        public void Startup()
        {
            list = new LinkedList<int>(values);
        }

        [Description("Test the count property.")]
        [TestMethod]
        public void Count()
        {
            Assert.AreEqual(list.Count, values.Length);
        }



        [Description("Test the CopyTo() method.")]
        [TestMethod]
        public void CopyTo()
        {
            int[] v = new int[values.Length / 2];
            list.CopyTo(v, 0);
            for (int i = 0; i < v.Length; i++)
            {
                Assert.AreEqual(values[i], v[i]);
            }
        }

        [Description("Test the Contains() method.")]
        [TestMethod]
        public void Contains()
        {
            Assert.IsTrue(list.Contains(values[0]));
            Assert.IsFalse(list.Contains(5000));
        }

        [Description("Test the Clear() method")]
        [TestMethod]
        public void Clear()
        {
            Assert.AreEqual(list.Count, values.Length);
            list.Clear();
            Assert.AreEqual(0, list.Count);
            list.Add(0);
            Assert.AreEqual(1, list.Count);
            list.Clear();
            Assert.AreEqual(0, list.Count);
        }

        //
        // AddLast(Node), AddLast(item)

        [Description("Test if Addlast() adds a null node")]
        [ExpectedException(typeof(ArgumentNullException), "Adding a null node")]
        [TestMethod]
        public void AddLast_LinkedNode_Null()
        {
            list.AddLast(null);
        }

        [Description("Test if AddLast(Node) throws exception for a node already on the list.")]
        [ExpectedException(typeof(InvalidOperationException), "Adding a node that is already on the list")]
        [TestMethod]
        public void AddLast_LinkedNode_FirstNode()
        {
            LinkedNode<int> first = list.First;
            list.AddLast(first);
        }

        [Description("Test if AddLast(item) adds the node and returns it correctly.")]
        [TestMethod]
        public void AddLast_Item_ReturnValue()
        {
            LinkedNode<int> last = list.AddLast(1);
            Assert.IsNotNull(last);
        }

        [Description("Test if AddLast(item) adds the data into the data structure.")]
        [TestMethod]
        public void AddLast_Item_Value1()
        {
            int value = 1;
            list.AddLast(value);
            Assert.AreEqual(list.Count, values.Length + 1);
            Assert.AreEqual(list.Last.Data, value);
        }

        [Description("Test if AddLast(item) adds the node and the value that returns is at the end of the list")]
        [TestMethod]
        public void AddLast_Item_2()
        {
            LinkedNode<int> last = list.AddLast(1);
            Assert.AreEqual(list.Last, last);
        }





        //
        // AddFirst(Node), AddFirst(item)

        [Description("Test if AddFirst() adds a null node.")]
        [ExpectedException(typeof(ArgumentNullException), "Adding a null node")]
        [TestMethod]
        public void AddFirst_LinkedNode_Null()
        {
            list.AddFirst(null);
        }

        [Description("Test if AddFirst(Node) throws exception for a node already on the list.")]
        [ExpectedException(typeof(InvalidOperationException), "Adding a node that is already on the list")]
        [TestMethod]
        public void AddFirst_LinkedNode_FirstNode()
        {
            LinkedNode<int> first = list.First;
            list.AddFirst(first);
        }

        [Description("Test if AddFirst(item) adds the node and returns it correctly.")]
        [TestMethod]
        public void AddFirst_Item_ReturnValue()
        {
            LinkedNode<int> last = list.AddFirst(1);
            Assert.IsNotNull(last);
        }

        [Description("Test if AddFirst(item) adds the data into the data structure.")]
        [TestMethod]
        public void AddFirst_Item_Value1()
        {
            int value = 1;
            list.AddFirst(value);
            Assert.AreEqual(list.Count, values.Length + 1);
            Assert.AreEqual(list.First.Data, value);
        }

        [Description("Test if AddFirst(item) adds the node and the value that returns is at the beggining of the list")]
        [TestMethod]
        public void AddFirst_Item_2()
        {
            LinkedNode<int> first = list.AddFirst(1);
            Assert.AreEqual(list.First, first);
        }







        //
        // AddAfter(Node, Node), AddAfter(Node, item)

        [Description("Test if AddAfter() throw exception for null node")]
        [ExpectedException(typeof(ArgumentNullException), "adding an item after a null node")]
        [TestMethod]
        public void AddAfter_LinkedNode_Item()
        {
            LinkedNode<int> n = list.AddAfter(null, 12345);
            Assert.IsNotNull(n);
        }


        [Description("Test if AddAfter(Node) throws exception for a node not on the list.")]
        [ExpectedException(typeof(InvalidOperationException), "Node is not on the list")]
        [TestMethod]
        public void AddAfter_LinkedNode_Item1()
        {
            LinkedNode<int> first = list.RemoveFirst();
            list.AddAfter(first, 12345);
        }

        [Description("Test if AddAfter() adds the node right after the first node")]
        [TestMethod]
        public void AddAfter_LinkedNode_Item2()
        {
            LinkedNode<int> n = list.AddAfter(list.First, 12345);
            Assert.AreEqual(list.First.Next, n);
        }


        [Description("Test if AddAfter() return the reference to the node created.")]
        [TestMethod]
        public void AddAfter_LinkedNode_Item3()
        {
            LinkedNode<int> n = list.AddAfter(list.First, 12345);
            Assert.IsNotNull(n);
        }


        [Description("Test if AddAfter() adds 2 nodes and one is next to another")]
        [TestMethod]
        public void AddAfter_LinkedNode_Item4()
        {
            LinkedNode<int> n = list.AddAfter(list.Last, 12345);
            LinkedNode<int> n2 = list.AddAfter(n, 54321);

            Assert.AreEqual(list.Last, n2);
            Assert.AreEqual(list.Last.Previous, n);
            Assert.AreEqual(n2, n.Next);
            Assert.AreEqual(n2.Previous, n);
        }



        //
        // AddBefore(Node, Node), AddBefore(Node, item)

        [Description("Test if AddBefore() throw exception for null node")]
        [ExpectedException(typeof(ArgumentNullException), "adding an item after a null node")]
        [TestMethod]
        public void AddBefore_LinkedNode_Item()
        {
            LinkedNode<int> n = list.AddBefore(null, 12345);
            Assert.IsNotNull(n);
        }


        [Description("Test if AddBefore(Node) throws exception for a node not on the list.")]
        [ExpectedException(typeof(InvalidOperationException), "Node is not on the list")]
        [TestMethod]
        public void AddBefore_LinkedNode_Item1()
        {
            LinkedNode<int> first = list.RemoveFirst();
            list.AddBefore(first, 12345);
        }

        [Description("Test if AddBefore() adds the node right after the first node")]
        [TestMethod]
        public void AddBefore_LinkedNode_Item2()
        {
            LinkedNode<int> n = list.AddBefore(list.First, 12345);
            Assert.AreEqual(list.First, n);
        }


        [Description("Test if AddBefore() return the reference to the node created.")]
        [TestMethod]
        public void AddBefore_LinkedNode_Item3()
        {
            LinkedNode<int> n = list.AddBefore(list.First, 12345);
            Assert.IsNotNull(n);
        }


        [Description("Test if AddBefore() adds 2 nodes and one is next to another")]
        [TestMethod]
        public void AddBefore_LinkedNode_Item4()
        {
            LinkedNode<int> n = list.AddBefore(list.Last, 12345);
            LinkedNode<int> n2 = list.AddBefore(n, 54321);

            Assert.AreEqual(list.Last.Previous, n);
            Assert.AreEqual(n.Previous, n2);
            Assert.AreEqual(n2.Next, n);
            Assert.AreEqual(n2, n.Previous);
        }






        //
        // RemoveFirst()

        [Description("Remove first element in the collection")]
        [TestMethod]
        public void RemoveFirst()
        {            
            LinkedNode<int> n = list.RemoveFirst();
            Assert.IsNotNull(n);
            Assert.AreNotEqual(n, list.First);
            Assert.IsNull(n.List);
            Assert.AreEqual(1, n.Data);
        }

        [Description("Remove first element in the collection")]
        [TestMethod]
        public void RemoveFirst_AllNodes()
        {
            var l = list.Count;
            for (int i = 0; i < l; i++)
            {
                list.RemoveFirst();                
            }

            Assert.IsNull(list.RemoveFirst());
        }




        //
        // RemoveLast()

        [Description("Remove last element in the collection")]
        [TestMethod]
        public void RemoveLast()
        {
            LinkedNode<int> n = list.RemoveLast();
            Assert.IsNotNull(n);
            Assert.AreNotEqual(n, list.Last);
            Assert.IsNull(n.List);
            Assert.AreEqual(1000, n.Data);
        }

        [Description("Remove last element in the collection")]
        [TestMethod]
        public void RemoveLast_AllNodes()
        {
            var l = list.Count;
            for (int i = 0; i < l; i++)
            {
                list.RemoveLast();
            }

            Assert.IsNull(list.RemoveLast());
        }



        //
        // RemoveAt(int)

        [Description("Remove element in the collection at index position 0")]
        [TestMethod]
        public void RemoveAt_0()
        {
            LinkedNode<int> n = list.RemoveAt(0);
            Assert.AreEqual(1, n.Data);
        }


        [Description("Remove element in the collection at index position -1")]
        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void RemoveAt_Minus1()
        {
            LinkedNode<int> n = list.RemoveAt(-1);
            Assert.AreEqual(1, n.Data);
        }

        [Description("Remove element in the collection at index position out of bounds")]
        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void RemoveAt_Count()
        {
            LinkedNode<int> n = list.RemoveAt(list.Count);
            Assert.AreEqual(1, n.Data);
        }




        //
        // Remove(Node)

        [Description("Remove null element")]
        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void Remove_null()
        {
            list.Remove(null);
        }

        [Description("Remove First element")]
        [TestMethod]
        public void Remove_First()
        {
            LinkedNode<int> first = list.First;
            Assert.AreEqual(first.List, list);

            list.Remove(list.First);
            Assert.IsNull(first.List);
        }

        [Description("Remove item with value 10")]
        [TestMethod]
        public void Remove_Item_10()
        {
            Assert.IsTrue(list.Contains(10));
            list.Remove(10);
            Assert.IsFalse(list.Contains(10));
        }






    }
}
