using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;

namespace MicroFE
{
    public class VideoWindow : OpenTK.GameWindow
    {

        Bitmap _backBuffer;
        bool _shouldRedraw;
        int _textureHandle;

        TextBuffer _buffer;
        List<TextMenu> _menuStack;
        List<TreeNode> _nodePath;
        TreeNode _root;

        MenuTheme _theme = null;


        TextBuffer _textBuffer;

        public VideoWindow(TreeNode root, MenuTheme theme) : base()
        {
            _buffer = new TextBuffer();
            _nodePath = new List<TreeNode>();

            _root = root; ConfigFileParser.ParseConfigFile("config.json");

            _theme = theme; ConfigFileParser.ParseMenuTheme("config.json");

            _menuStack = new List<TextMenu>();
            var tmView = new TextMenu(_buffer, 0, 0, TextBuffer.TextCols, TextBuffer.TextRows, "[Micro FE - Main Menu]", _theme)
            {
                Items = _root.Keys.ToArray(),
                SelectedIndex = 0,
            };

            _nodePath.Add(_root);
            _menuStack.Add(tmView);
        }

        void CreateTexture()
        {

            GL.GenTextures(1, out _textureHandle);
            GL.BindTexture(TextureTarget.Texture2D, _textureHandle);

        }

        void UpdateTexture()
        {
            var data = _backBuffer.LockBits(new System.Drawing.Rectangle(0, 0, _backBuffer.Width, _backBuffer.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            _backBuffer.UnlockBits(data);


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (!this.Focused) { return; }

            if ((OpenTK.Input.GamePad.GetState(0).DPad.Up == ButtonState.Pressed))
            {
                _menuStack.Last().MoveCursorUp();
                _shouldRedraw = true;
            }
            if ((OpenTK.Input.GamePad.GetState(0).DPad.Down == ButtonState.Pressed))
            {
                _menuStack.Last().MoveCursorDown();
                _shouldRedraw = true;
            }
            if ((OpenTK.Input.GamePad.GetState(0).DPad.Left == ButtonState.Pressed))
            {
                if (_menuStack.Count > 1)
                {
                    _menuStack.Remove(_menuStack.Last());
                    _nodePath.Remove(_nodePath.Last());
                    _shouldRedraw = true;

                }
            }

            if ((OpenTK.Input.GamePad.GetState(0).DPad.Right == ButtonState.Pressed))
            {
                HandlePropertyActivated();
                _shouldRedraw = true;

            }
            base.OnUpdateFrame(e);
        }


        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                _menuStack.Last().MoveCursorUp();
            }

            if (e.Key == Key.Down)
            {
                _menuStack.Last().MoveCursorDown();
            }

            if (e.Key == Key.Left)
            {
                if (_menuStack.Count > 1)
                {
                    _menuStack.Remove(_menuStack.Last());
                    _nodePath.Remove(_nodePath.Last());
                }
            }

            if (e.Key == Key.Right)
            {
                HandlePropertyActivated();
            }

            _shouldRedraw = true;
            base.OnKeyUp(e);
        }


        private void HandlePropertyActivated()
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
                    TextBuffer.TextCols - _nodePath.Count - 1,
                    TextBuffer.TextRows,
                    currentMenu.Items[currentMenu.SelectedIndex], _theme);
                tm.Items = nextNode.Keys.ToArray();
                _menuStack.Add(tm);
            }
        }





        protected override void OnLoad(EventArgs e)
        {
            _backBuffer = new Bitmap(this.Width, this.Height);
            _textBuffer = new TextBuffer();
            CreateTexture();

            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);

            base.OnLoad(e);
        }

        protected override void OnResize(EventArgs e)
        {
            _backBuffer = new Bitmap(Width, Height);
            _shouldRedraw = true;
            GL.Viewport(0, 0, Width, Height);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            if (_shouldRedraw)
            {
                using (var ctx = Graphics.FromImage(_backBuffer))
                {
                    _menuStack.ForEach(d => d.Draw());
                    _buffer.RenderTextBuffer(ctx, new Rectangle(0, 0, Width, Height));

                    UpdateTexture();
                    _shouldRedraw = false;
                }
            }

            GL.Begin(PrimitiveType.Quads);

            GL.TexCoord2(0, 1); GL.Vertex2(-1, -1);
            GL.TexCoord2(0, 0); GL.Vertex2(-1, 1);
            GL.TexCoord2(1, 0); GL.Vertex2(1, 1);
            GL.TexCoord2(1, 1); GL.Vertex2(1, -1);


            GL.End();

            SwapBuffers();

            base.OnRenderFrame(e);
        }
    }
}
