using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace FirstPrincipals2
{
    class Ship:Entity
    {
        Model model;
        Vector3 velocity, force, acceleration;
        Vector3 angularVelocity, angularAcceleration, torque;
        Vector3 basis;
        Matrix inertialTensor;
        Quaternion quaternion;

        float mass;

        public Ship()
        {
            mass = 10.0f;
            velocity = Vector3.Zero;
            force = Vector3.Zero;
            acceleration = Vector3.Zero;
            angularAcceleration = Vector3.Zero;
            angularVelocity = Vector3.Zero;
            torque = Vector3.Zero;
            basis = Vector3.Forward;
            quaternion = Quaternion.Identity;

        }

        public override void LoadContent()
        {
            model = Game1.instance.Content.Load<Model>("fighter");
            calculateInertiaTensor();
        }



        private void calculateInertiaTensor()
        {
            BoundingBox box = new BoundingBox();
            foreach (ModelMesh mesh in model.Meshes)
            {
                box = BoundingBox.CreateMerged(box, BoundingBox.CreateFromSphere(mesh.BoundingSphere));
            }
	        float width = box.Max.X - box.Min.X;
	        float height = box.Max.Y - box.Min.Y;
	        float depth = box.Max.Z - box.Min.Z;


            inertialTensor.M11 = (float) (mass * (Math.Pow(height, 2) + Math.Pow(depth, 2))) / 12.0f;
            inertialTensor.M22 = (float) (mass * (Math.Pow(width, 2) + Math.Pow(depth, 2))) / 12.0f;
            inertialTensor.M33 = (float) (mass * (Math.Pow(width, 2) + Math.Pow(height, 2))) / 12.0f;
	        inertialTensor.M44 = 1;
        }

        void addForce(Vector3 force)
        {
            this.force += force;
        }

        void addTorque(Vector3 torque)
        {
            this.torque += torque;
        }

        void addForceAtPoint(Vector3 force, Vector3 point)
        {
            Vector3 to = Vector3.Cross(force, point);
            torque += to;

            force += force;
        }

        public override void Update(GameTime gameTime)
        {
            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Space))
            {
                addForce(look * 10.0f);
            }

            // Yaw
            if (state.IsKeyDown(Keys.J))
            {
                addTorque(this.up * 10.0f);
            }
            if (state.IsKeyDown(Keys.L))
            {
                addTorque(this.up * -10.0f);
            }
            // End of Yaw

            //Pitch
            if (state.IsKeyDown(Keys.I))
            {
                addTorque(this.right * 10.0f);
            }
            if (state.IsKeyDown(Keys.K))
            {
                addTorque(this.right * -10.0f);
            }
            // End of Pitch

            if (state.IsKeyDown(Keys.Y))
            {
                addTorque(this.look * 10.0f);
            }

            if (state.IsKeyDown(Keys.H))
            {
                addTorque(this.look * -10.0f);
            }

            // Do the Newtonian integration
            acceleration = force / mass;
            velocity += acceleration * timeDelta;
            pos += velocity * timeDelta;
            force = Vector3.Zero;

            if (velocity.Length() > 0.0001f)
            {
                look = Vector3.Normalize(velocity);
                right = Vector3.Cross(look, up);
                velocity *= 0.99f;
            }

            // Do the Hamiltonian integration
            angularAcceleration = Vector3.Transform(torque, Matrix.Invert(inertialTensor));
            angularVelocity = angularVelocity + angularAcceleration * timeDelta;

            Quaternion w = new Quaternion(angularVelocity.X, angularVelocity.Y, angularVelocity.Z, 0);
            
            quaternion += ((w * (timeDelta / 2.0f)) * quaternion);
            quaternion.Normalize();
            torque = Vector3.Zero;

            // Recalculate the Look, up and right
            w = new Quaternion(Vector3.Forward, 0);
            w = quaternion * w * Quaternion.Inverse(quaternion);
            look.X = w.X;
            look.Y = w.Y;
            look.Z = w.Z;
            w = new Quaternion(Vector3.Up, 0);
            w = quaternion * w * Quaternion.Inverse(quaternion);
            up.X = w.X;
            up.Y = w.Y;
            up.Z = w.Z;

            w = new Quaternion(Vector3.Right, 0);
            w = quaternion * w * Quaternion.Inverse(quaternion);
            right.X = w.X;
            right.Y = w.Y;
            right.Z = w.Z;

        }

        public override void Draw(GameTime gameTime)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = Game1.instance.Camera.View;
                    effect.Projection = Game1.instance.Camera.Projection;
                    effect.World = Matrix.CreateFromQuaternion(quaternion) * Matrix.CreateTranslation(pos);
                    effect.EnableDefaultLighting();                    
                }
                mesh.Draw();
            }
        }
    }
}
