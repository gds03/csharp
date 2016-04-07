using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Algorithms.Collections.Generic
{
    public class AVL<T>
        where T : IComparable<T>
    {
        private enum State { LH, EH, RH }

        class Node<T>
        {
            public T val { get; internal protected set; }
            public Node<T> left { get; internal protected set; }
            public Node<T> right { get; internal protected set; }
            public State state { get; internal protected set; }

            public Node(T v)
            {
                val = v;
                left = right = null;
                state = State.EH;
            }
        }


        private Node<T> root;



        //
        // private methods

        private Node<T> rotateRight(Node<T> root)
        {
            //actualizar estados
            if (root.state == State.LH) root.state = State.EH;

            if (root.left.state == State.EH) root.left.state = State.RH;
            else if (root.left.state == State.LH) root.left.state = State.EH;


            Node<T> aux = root.left;
            if (aux == null) return root;
            root.left = aux.right;
            aux.right = root;

            return aux;
        }

        private Node<T> rotateLeft(Node<T> root)
        {
            if (root.state == State.RH) root.state = State.EH;

            if (root.right.state == State.EH) root.right.state = State.LH;
            else if (root.right.state == State.RH) root.right.state = State.EH;

            Node<T> aux = root.right;
            if (aux == null) return root;
            root.right = aux.left;
            aux.left = root;
            return aux;
        }




        private Node<T> doubleRotateRight(Node<T> root)
        {
            root.left = rotateLeft(root.left);
            Node<T> aux = rotateRight(root);
            aux.state = State.EH;
            aux.left.state = State.EH;
            aux.right.state = State.EH;
            return aux;
        }

        private Node<T> doubleRotateLeft(Node<T> root)
        {
            root.right = rotateRight(root.right);
            Node<T> aux = rotateLeft(root);
            aux.state = State.EH;
            aux.left.state = State.EH;
            aux.right.state = State.EH;
            return aux;
        }






        private Node<T> insertBalancedR(Node<T> root, Node<T> n)
        {
            if (n.val.CompareTo(root.val) > 0)
            {
                if (root.right == null)
                {
                    root.right = n;
                    if (root.state == State.EH) root.state = State.RH;
                    else if (root.state == State.LH) root.state = State.EH;
                    else return rotateLeft(root);
                }
                else
                {
                    State lastState = root.right.state;
                    root.right = insertBalancedR(root.right, n);
                    if (lastState == State.EH && root.right.state != State.EH)
                    {
                        if (root.state == State.EH) root.state = State.RH;
                        else if (root.state == State.LH) root.state = State.EH;
                        else
                        {
                            if (root.right.state == State.LH) return doubleRotateLeft(root);
                            else return rotateLeft(root);
                        }
                    }
                }
            }
            else
            {
                if (root.left == null)
                {
                    root.left = n;
                    if (root.state == State.EH) root.state = State.LH;
                    else if (root.state == State.RH) root.state = State.EH;
                    else return rotateRight(root);
                }
                else
                {
                    State lastState = root.left.state;
                    root.left = insertBalancedR(root.left, n);
                    if (lastState == State.EH && root.left.state != State.EH)
                    {
                        if (root.state == State.EH) root.state = State.LH;
                        else if (root.state == State.RH) root.state = State.EH;
                        else
                        {
                            if (root.left.state == State.RH) return doubleRotateRight(root);
                            else return rotateRight(root);
                        }
                    }
                }

            }
            return root;
        }


        public void insertBalanced(T value)
        {
            Node<T> n = new Node<T>(value);
            root = (root != null) ? insertBalancedR(root, n) : n;
        }



        void printNode(int level, T val)
        {
            for (int i = 0; i < level; ++i)
                Console.Write("\t");
            Console.WriteLine(val);
        }

        void print(Node<T> h, int level)
        {
            if (h == null) return;
            print(h.right, level + 1);
            printNode(level, h.val);
            print(h.left, level + 1);
        }

        void print()
        {
            print(root, 0);
        }

    }
}
