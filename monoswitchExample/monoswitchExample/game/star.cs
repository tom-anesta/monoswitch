using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace monoswitchExample
{
    class star
    {

        #region members

            #region public

            #endregion

            #region protected

                
                // The position of the star relative to the top left corner of thescreen
                protected Vector2 m_position;
                protected Vector2 m_altPosition;
                //the alternative position for when the star is outside the bounds
                // Animation representing the star
                protected Animation m_starAnimation;
                protected Animation m_altStarAnimation;//the alternative animation for when the star is outside the bounds
                // The state of the star
                protected bool m_active;
                // The hit points of the star, if this goes to zero (player hits star) the star is collected
                protected int m_health;
                // The amount of damage the star inflicts on the player ship
                //protected int m_damage;
                // The amount of score the star will give to the player
                protected int m_value;
                // The speed at which the star moves
                //float m_moveSpeed;

            #endregion

            #region private

            #endregion

        #endregion

        #region properties

            #region public

                public bool active
                {
                    get
                    {
                        return this.m_active;
                    }
                }

                public int value
                {
                    get
                    {
                        return this.m_value;
                    }
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
                        return (this.m_altStarAnimation != null);
                    }
                }

                public Rectangle colRect
                {
                    get
                    {
                        return new Rectangle();
                    }
                }

                public Rectangle altColRect
                {
                    get
                    {
                        return new Rectangle();
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

                // Get the width of the enemy ship
                public int Width
                {
                    get { return this.m_starAnimation.frameWidth; }
                }

                // Get the height of the enemy ship
                public int Height
                {
                    get { return this.m_starAnimation.frameHeight; }
                }

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
                    // Set the position of the enemy
                    this.m_position = position;
                    this.m_altPosition = Vector2.Zero;//non-nullable
                    // Load the enemy ship texture
                    this.m_starAnimation = animation;
                    this.m_altStarAnimation = null;
                    // We initialize the enemy to be active so it will be update in the game
                    this.m_active = true;
                    // Set the health of the enemy
                    this.m_health = 10;
                    // Set the amount of damage the enemy can do
                    //this.m_damage = 10;
                    // Set how fast the enemy moves
                    //this.m_moveSpeed = 6f;
                    // Set the score value of the enemy
                    this.m_value = 100;
                }

                public void Update(GameTime gameTime)
                {
                    // Update the position of the Animation
                    this.m_starAnimation.position = this.m_position;
                    // Update Animation
                    this.m_starAnimation.Update(gameTime);
                    if (this.m_altStarAnimation != null)
                    {
                        this.m_altStarAnimation.Update(gameTime);
                    }
                }

                public void Draw(SpriteBatch spriteBatch)
                {
                    // Draw the animation
                    this.m_starAnimation.Draw(spriteBatch);
                    if (this.m_altStarAnimation != null)
                    {
                        this.m_starAnimation.Draw(spriteBatch);
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
