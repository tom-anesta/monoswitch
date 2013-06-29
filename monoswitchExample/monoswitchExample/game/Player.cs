using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace monoswitchExample
{
    class Player
    {

        #region members

            #region public

            #endregion

            #region protected

                // Position of the Player relative to the upper left side of the screen
                protected Vector2 m_position;
                protected Vector2 m_altPosition;//the position outside of the screen
                // State of the player
                protected bool m_active;
                // Amount of hit points that player has
                protected int m_health;
                // Animation representing the player
                protected Animation m_playerAnimation;
                protected Animation m_altPlayerAnimation;//the position outside of the screen

            #endregion

            #region private

            #endregion

        #endregion

        #region properties

            #region public

                // Get the width of the player ship
                public int Width
                {
                    get { return this.m_playerAnimation.frameWidth; }
                }

                // Get the height of the player ship
                public int Height
                {
                    get { return this.m_playerAnimation.frameHeight; }
                }

                public int health
                {
                    get
                    {
                        return this.m_health;
                    }
                    set
                    {
                        this.m_health = value;
                    }
                }

                public Vector2 position
                {
                    get
                    {
                        return this.m_position;
                    }
                }

                public bool isOutOfBounds
                {
                    get
                    {
                        return (this.m_altPlayerAnimation != null);
                    }
                }

                public bool active
                {
                    get
                    {
                        return this.m_active;
                    }
                    set
                    {
                        this.m_active = value;
                    }
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

                public void Initialize(Animation animation, Vector2 position)
                {
                    

                    // Set the starting position of the player around the middle of the screen and to the back
                    this.m_position = position;
                    this.m_altPosition = Vector2.Zero;
                    this.m_playerAnimation = animation;
                    this.m_altPlayerAnimation = null;
                    // Set the player to be active
                    this.m_active = true;
                    // Set the player health
                    this.m_health = 100;
                }

                public void Update(GameTime gameTime)
                {
                    this.m_playerAnimation.position = this.m_position;
                    if (this.m_altPlayerAnimation != null)
                    {
                        this.m_altPlayerAnimation.position = this.m_altPosition;
                    }
                    this.m_playerAnimation.Update(gameTime);
                }

                public void Draw(SpriteBatch spriteBatch)
                {
                    this.m_playerAnimation.Draw(spriteBatch);
                    if (this.m_altPlayerAnimation != null)
                    {
                        this.m_playerAnimation.Draw(spriteBatch);
                    }
                }

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion


    }
}
