﻿using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Input;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;

namespace MicroFE
{
    /// <summary>
    /// Video Window Class
    /// 
    /// Primary experience for MicroFE
    /// </summary>
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


            _buffer = new TextBuffer(Program.Settings.Cols, Program.Settings.Rows);
            _nodePath = new List<TreeNode>();

            _root = root;

            _theme = theme; 

            _menuStack = new List<TextMenu>();
            var tmView = new TextMenu(_buffer, 0, 0, _buffer.TextCols, _buffer.TextRows, "[Micro FE - Main Menu]", _theme)
            {
                Items = _root.Keys.ToArray(),
                SelectedIndex = 0,
            };

            _nodePath.Add(_root);
            _menuStack.Add(tmView);
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

        /// <summary>
        /// Use this method to handle gamepad input.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            
            if (!this.Focused)
            {

                if (Program.RunningEmulator != null)
                {
                    var state = GamePad.GetState(0);
                    bool shouldKill = false;
                    switch (Program.Settings.QuitCombo)
                    {
                        case ("L3+R3"):
                            shouldKill = (state.Buttons.LeftStick == ButtonState.Pressed && state.Buttons.RightStick == ButtonState.Pressed);

                            break;
                    }

                    if (shouldKill)
                    {
                        try
                        {
                            Program.RunningEmulator.Kill();
                        }
                        catch { }

                        finally
                        {
                            Program.RunningEmulator = null;
                        }
                    }
                }

            }
            else
            {

            if ((GamePad.GetState(0).DPad.Up == ButtonState.Pressed))
            {
                _menuStack.Last().MoveCursorUp();
                _shouldRedraw = true;
            }
            if ((GamePad.GetState(0).DPad.Down == ButtonState.Pressed))
            {
                _menuStack.Last().MoveCursorDown();
                _shouldRedraw = true;
            }
            if ((GamePad.GetState(0).DPad.Left == ButtonState.Pressed))
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

            if (nextNode == null) return;

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
                    _buffer.TextCols - _nodePath.Count - 1,
                    _buffer.TextRows,
                    currentMenu.Items[currentMenu.SelectedIndex], _theme);
                tm.Items = nextNode.Keys.ToArray();
                _menuStack.Add(tm);
            }
        }




        /// <summary>
        /// Setup the graphics.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            _backBuffer = new Bitmap(this.Width, this.Height);

            GL.GenTextures(1, out _textureHandle);
            GL.BindTexture(TextureTarget.Texture2D, _textureHandle);

            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);

            base.OnLoad(e);
        }

        /// <summary>
        /// Handle a window resize event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            _backBuffer = new Bitmap(Width, Height);
            _shouldRedraw = true;
            GL.Viewport(0, 0, Width, Height);
        }

        /// <summary>
        /// Draw the menu stack every frame.
        /// If should redraw has been flagged, the texture will be regenerated.
        /// </summary>
        /// <param name="e"></param>
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
