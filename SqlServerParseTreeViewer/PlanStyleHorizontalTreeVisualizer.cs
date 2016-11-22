﻿//  Copyright(c) 2016 Brian Hansen.

//  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//  documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//  the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
//  and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

//  The above copyright notice and this permission notice shall be included in all copies or substantial portions 
//  of the Software.

//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//  TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//  THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//  CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
//  DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bkh.ParseTreeLib;

namespace SqlServerParseTreeViewer
{
    public class PlanStyleHorizontalTreeVisualizer : TreeVisualizer
    {
        private const int _iconWidth = 25;
        private const int _iconHeight = 25;
        private const int _interIconHorizontalSpacing = 50;
        private const int _interIconVerticalSpacing = 30;
        private const int _iconToTextSpacing = 5;
        private const int _leftMargin = 5;
        private const int _rightMargin = 5;
        private const int _topMargin = 5;
        private const int _bottomMargin = 5;

        private Graphics _dummyGraphics;
        private Font _textFont = new Font("Times New Roman", 10);

        public PlanStyleHorizontalTreeVisualizer()
        {
        }

        public override Bitmap Render(SqlParseTree tree, out List<TreeNodeIcon> nodeIcons)
        {
            if (tree == null)
            {
                throw new ArgumentNullException(nameof(tree));
            }

            using (Bitmap dummyBitmap = new Bitmap(1, 1))
            {
                _dummyGraphics = Graphics.FromImage(dummyBitmap);
                try
                {

                    nodeIcons = PositionNodes(tree);
                }
                finally
                {
                    _dummyGraphics.Dispose();
                }
            }

            int width = _leftMargin + nodeIcons.GetWidth() + _rightMargin;
            int height = _topMargin + nodeIcons.GetHeight() + _bottomMargin;

            Bitmap bitmap = new Bitmap(width, height);

            return bitmap;
        }

        private List<TreeNodeIcon> PositionNodes(SqlParseTree tree)
        {
            List<TreeNodeIcon> nodeIcons = PositionSubtree(tree.RootNode, _leftMargin, _topMargin);

            // TODO: Normalize positions
            return nodeIcons;
        }

        private List<TreeNodeIcon> PositionSubtree(SqlParseTreeNode head, int top, int left)
        {
            List<TreeNodeIcon> icons = new List<TreeNodeIcon>();

            //// Find the right-most leaf node.
            //SqlParseTreeNode currentNode = head;
            //while (currentNode.Children.Count > 0)
            //{
            //    currentNode = currentNode.Children[currentNode.Children.Count - 1];
            //}

            //// Position it
            //TreeNodeIcon icon = PositionNode(currentNode, top, left);
            //icons.Add(icon);

            TreeNodeIcon icon = PositionNode(head, top, left);

            //while (currentNode.Parent != null &&
            //    currentNode != head)
            //{
            //    SqlParseTreeNode previousNode = currentNode;
            //    currentNode = currentNode.Parent;

            //    List<TreeNodeIcon> parentIcons = PositionSubtree(currentNode, 0, 0);

            //    // Position other children of previous node
            //    List<SqlParseTreeNode> siblings = currentNode.Children.Where(n => n != previousNode).ToList();
            //    foreach (SqlParseTreeNode node in siblings)
            //    {
            //        // Position it
            //    }

            //    // Position currentNode;
            //}

            int newLeft = left + icon.Width + _interIconHorizontalSpacing;
            int newTop = top + icon.Height + _interIconVerticalSpacing;

            foreach (SqlParseTreeNode child in head.Children)
            {
                List<TreeNodeIcon> childNodes = PositionSubtree(child, newTop, newLeft);
                if (childNodes.Count > 0)
                {
                    childNodes[0].Parent = icon;
                    icon.Children.Add(childNodes[0]);
                }
                newTop += childNodes.GetHeight();
            }

            return icons;
        }

        private TreeNodeIcon PositionNode(SqlParseTreeNode node, int top, int left)
        {
            string nodeText = node.ToString();
            SizeF textSize = _dummyGraphics.MeasureString(nodeText, _textFont);

            TreeNodeIcon icon = new TreeNodeIcon();
            icon.X = left;
            icon.Y = top;
            icon.Width = (int)Math.Max(_iconWidth, textSize.Width);
            icon.Height = _iconHeight + _iconToTextSpacing + (int)textSize.Height;
            icon.Node = node;
            icon.Text = nodeText;
            icon.TextRectangle = new Rectangle((icon.Width - (int)textSize.Width) / 2, _iconHeight + _iconToTextSpacing, (int)textSize.Width, (int)textSize.Height);
            icon.IconRectangle = new Rectangle((icon.Width - _iconWidth) / 2, 0, _iconWidth, _iconHeight);

            return icon;
        }
    }
}
