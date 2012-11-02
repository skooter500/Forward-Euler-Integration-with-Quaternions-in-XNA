using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace FirstPrincipals2
{
    abstract class Entity
    {
        public Vector3 pos;
        public Vector3 up;
        public Vector3 look;
        public Vector3 right;

        public Entity()
        {
            pos = Vector3.Zero;
            look = Vector3.Forward;
            up = Vector3.Up;
            right = Vector3.Right;
        }

        public abstract void Update(GameTime gameTime);

        public virtual void Draw(GameTime gameTime)
        {
        }

        public virtual void LoadContent()
        {

        }
        
        public void walk(float units)
        {
            pos += units * look;
        }

        public void strafe(float units)
        {
            pos += units * right;
        }

        public void yaw(float units)
        {
            Matrix rotMatrix = Matrix.CreateRotationY(units);

            look = Vector3.Transform(look, rotMatrix);
            right = Vector3.Transform(right, rotMatrix);
        }

        public void pitch(float units)
        {
            Matrix rotMatrix = Matrix.CreateFromAxisAngle(right, units);

            look = Vector3.Transform(look, rotMatrix);
            up = Vector3.Transform(up, rotMatrix);

        }
    }
}
