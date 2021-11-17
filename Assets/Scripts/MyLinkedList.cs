using System;
using System.Collections.Generic;
using UnityEngine;

namespace AbstractDataTypes
{
    public class MyListNode<T>
    {
        public MyListNode<T> next;
        public MyListNode<T> prev;
        public T data;

        public MyListNode(T newData = default)
        {
            data = newData;
        }
    }
    
    public class MyLinkedList<T>
    {
        private MyListNode<T> head;
        private MyListNode<T> tail;
        private int count;

        public int Count => count;

        //Constructor that always gets called when we use "new MyLinkedList()", here we set the initial values
        public MyLinkedList()
        {
            head = null;
            tail = null;
            count = 0;
        }

        public void AddNode(T value)
        {
            //Run this code when the first node is created
            if (head == null)
            {
                head = new MyListNode<T>{data = value};
                tail = head;
                count++;
                return;
            }
            
            MyListNode<T> curNode = tail;       //Temp node

            curNode.next = new MyListNode<T>(); //Create the new node
            curNode.next.data = value;          //Assign the data to the new node
            curNode.next.prev = curNode;        //Point back to current node in the next node..
            tail = curNode.next;                //Set the new element to be the tail since it will be the last element
            count++;                            //Update count
        }
        
        public void AddLast(MyListNode<T> node)
        {
            //Run this code when the first node is created
            if (head == null)
            {
                head = node;
                tail = head;
                count++;
                return;
            }
            
            MyListNode<T> curNode = tail;       //Temp node
            curNode.next = node; //Create the new node
            curNode.next.prev = curNode;        //Point back to current node in the next node..
            tail = curNode.next;                //Set the new element to be the tail since it will be the last element
            count++;                            //Update count
        }

        public void PrintNodes()
        {
            MyListNode<T> curNode = head;
            for (int i = 0; i < Count; i++)
            {
                Console.WriteLine(curNode.data);
                if (curNode.next != null)
                {
                    curNode = curNode.next;
                }
            }
        }
        public void PrintNodesReversed()
        {
            MyListNode<T> curNode = tail;
            for (int i = 0; i < Count; i++)
            {
                Console.WriteLine(curNode.data);
                if (curNode.prev != null)
                {
                    curNode = curNode.prev;
                }
            }
        }

        public void InsertNode(int index, T value)
        {
            //IMPORTANT: Index 0 would be inserting the node between the head and the first link. Not sure if index 0 should be the same as "AddFirst"
            //todo >>MAYBE<< We might need to just make this func call 'AddFirst' or 'AddLast' if we give it index 0 or Index = Count
            
            if (index < 0 || index >  count - 1) return; //Early return if we give negative index
            
            MyListNode<T> curNode = head;
            
            for (int i = 0; i < index; i++)
            {
                if (curNode.next != null)
                {
                    curNode = curNode.next;
                }
            }

            MyListNode<T> nodeToInsert = new MyListNode<T>();

            nodeToInsert.next = curNode.next;       //Points the new node to the next node
            nodeToInsert.next.prev = nodeToInsert;  //Point the next node back to this node
            nodeToInsert.prev = curNode;            //Point to the previous node

            curNode.next = nodeToInsert;            //Would be the same as nodeToInsert.prev.next, but since we already have curNode we can use that instead.
            nodeToInsert.data = value;              //Assign the new list node with data
            count++;                                //Increment count since we added a new node
        }

        public T GetFirstNode()
        {
           return head.data;
        }
        
        public T GetLastNode()
        {
            return tail.data;
        }
        
        public MyListNode<T> First()
        {
            return head;
        }
        
        public MyListNode<T> Last()
        {
            return tail;
        }

        public T GetNthNode(int index)
        {
            if (index < 0 || index > count - 1) return default; //Early return if we give negative index
            
            MyListNode<T> curNode = head;
            
            for (int i = 0; i < index; i++)
            {
                if (curNode.next != null)
                {
                    curNode = curNode.next;
                }
                else
                {
                    return default;
                }
            }
            return curNode.data;
        }
        
        public int Find(T value)
        {
            int findIndex = 0;
            MyListNode<T> curNode = head;
            
            //Go through the linked list and see if we find a match
            while (true)
            {
                if (curNode.next != null)
                {
                    if (value.Equals(curNode.data)) //Can't use == operator so we use Equals instead (Logically the same as value == curNode.data)
                    {
                        return findIndex;
                    }
                    curNode = curNode.next;
                }
                else
                {
                    return -1;
                }
            }
        }

        public T this[int index]
        {
            get { return GetNthNode(index); }
            set { throw new NotImplementedException(); }
        }
    }
}