using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MicroFE
{
    public partial class MainForm : Form
    {

        TextBuffer _buffer;
        List<TextMenu> _menuStack;
        List<TreeNode> _nodePath;
        TreeNode _root;

        public MainForm()
        {
            InitializeComponent();

            _buffer = new TextBuffer();
            _nodePath = new List<TreeNode>();

            _root = ConfigFileParser.ParseConfigFile("config.json");
           


            _menuStack = new List<TextMenu>();
            var tmView = new TextMenu(_buffer, 0, 0, TextBuffer.TextCols, TextBuffer.TextRows, "[Micro FE - Main Menu]")
            {
                Items = _root.Keys.ToArray(),
                SelectedIndex = 0,
            };

            _nodePath.Add(_root);
            _menuStack.Add(tmView);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                _menuStack.Last().MoveCursorUp();
            }

            if (e.KeyCode == Keys.Down)
            {
                _menuStack.Last().MoveCursorDown();
            }

            if (e.KeyCode == Keys.Left)
            {
                if (_menuStack.Count > 1)
                {
                    _menuStack.Remove(_menuStack.Last());
                    _nodePath.Remove(_nodePath.Last());
                }
            }

            if (e.KeyCode == Keys.Right)
            {
                var currentMenu = _menuStack.Last();
                var nextNode = _nodePath.Last()[currentMenu.Items[currentMenu.SelectedIndex]];

                if (nextNode.OnSelect != null)
                {
                    nextNode.OnSelect();
                }
                else if (nextNode.Any())
                {
                    _nodePath.Add(nextNode);
                    var tm = new TextMenu(_buffer,
                        _nodePath.Count + 1,
                        0,
                        TextBuffer.TextCols - _nodePath.Count -1,
                        TextBuffer.TextRows ,
                        currentMenu.Items[currentMenu.SelectedIndex]);
                    tm.Items = nextNode.Keys.ToArray();
                    _menuStack.Add(tm);
                }
            }

            Invalidate();
            base.OnKeyUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {

            _menuStack.ForEach(d => d.Draw());
            try
            {
                _buffer.RenderTextBuffer(e.Graphics, new Rectangle(0, 0, Width, Height));
            }
            catch { }
            base.OnPaint(e);
        }




    }
}

