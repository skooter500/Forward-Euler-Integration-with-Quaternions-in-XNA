using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace FirstPrincipals2
{
    class Camera:Entity
    {
        private Matrix view;

        public Matrix View
        {
            get { return view; }
            set { view = value; }
        }

        private Matrix projection;

        public Matrix Projection
        {
            get { return projection; }
            set { projection = value; }
        }

        public override void Update(GameTime gameTime)
        {
            float speed = 5.0f;
            float timeDelta = (float) gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.W))
            {
                walk(speed * timeDelta);
            }
            if (state.IsKeyDown(Keys.S))
            {
                walk(-speed * timeDelta);
            }
            if (state.IsKeyDown(Keys.A))
            {
                strafe( - speed * timeDelta);
            }
            if (state.IsKeyDown(Keys.D))
            {
                strafe(speed * timeDelta);
            }

            MouseState mouseState = Mouse.GetState();

            int mouseX = mouseState.X;
            int mouseY = mouseState.Y;

            int midX = GraphicsDeviceManager.DefaultBackBufferHeight / 2;
            int midY = GraphicsDeviceManager.DefaultBackBufferWidth / 2;

            int deltaX = mouseX - midX;
            int deltaY = mouseY - midY;

            yaw(-(float)deltaX / 100.0f);
            pitch(-(float)deltaY / 100.0f);
            Mouse.SetPosition(midX, midY);

            view = Matrix.CreateLookAt(pos, pos + look, Vector3.Up);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 8.0f / 6.0f, 0.1f, 10000.0f);

        }
    }
}
