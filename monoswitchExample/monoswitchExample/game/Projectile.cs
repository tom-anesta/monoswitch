// Projectile.cs
//Using declarations
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace monoswitchExample
{
    class Projectile
    {

        #region members

            #region public

            #endregion

            #region protected

                // Image representing the Projectile
                protected Texture2D m_texture;

                // Position of the Projectile relative to the upper left side of the screen
                protected Vector2 m_position;

                // State of the Projectile
                protected bool m_active;

                // The amount of damage the projectile can inflict to an enemy
                protected int m_damage;

                // Represents the viewable boundary of the game
                protected Viewport m_viewport;

                // Determines how fast the projectile moves
                protected float m_projectileMoveSpeed;

            #endregion

            #region private

            #endregion

        #endregion

        #region properties

            #region public

                // Get the width of the projectile ship
                public int Width
                {
                    get { return m_texture.Width; }
                }

                // Get the height of the projectile ship
                public int Height
                {
                    get { return m_texture.Height; }
                }

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

        #region events

            #region public

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

        #region functions

            #region public

                public void Initialize(Viewport viewport, Texture2D texture, Vector2 position)
                {

                    this.m_texture = texture;
                    this.m_position = position;
                    this.m_viewport = viewport;

                    this.m_active = true;

                    this.m_damage = 2;

                    this.m_projectileMoveSpeed = 20f;

                }

                public void Update()
                {
                    // Projectiles always move to the right
                    this.m_position.X += this.m_projectileMoveSpeed;

                    // Deactivate the bullet if it goes out of screen
                    if (this.m_position.X + this.m_texture.Width / 2 > this.m_viewport.Width)
                        this.m_active = false;
                }

                public void Draw(SpriteBatch spriteBatch)
                {

                    spriteBatch.Draw(this.m_texture, this.m_position, null, Color.White, 0f, new Vector2(this.Width / 2, this.Height / 2), 1f, SpriteEffects.None, 0f);

                }

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion



        

        

        


        

    }
}
