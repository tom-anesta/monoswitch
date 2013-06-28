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

                // Animation representing the enemy
                protected Animation m_enemyAnimation;

                // The position of the enemy ship relative to the top left corner of thescreen
                protected Vector2 m_position;

                // The state of the Enemy Ship
                protected bool m_active;

                // The hit points of the enemy, if this goes to zero the enemy dies
                protected int m_health;

                // The amount of damage the enemy inflicts on the player ship
                protected int m_damage;

                // The amount of score the enemy will give to the player
                protected int m_value;

                // The speed at which the enemy moves
                float m_moveSpeed;

            #endregion

            #region private

            #endregion

        #endregion

        #region properties

            #region public

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
                    get { return this.m_enemyAnimation.frameWidth; }
                }

                // Get the height of the enemy ship
                public int Height
                {
                    get { return this.m_enemyAnimation.frameHeight; }
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
                    // Load the enemy ship texture
                    this.m_enemyAnimation = animation;
                    // Set the position of the enemy
                    this.m_position = position;
                    // We initialize the enemy to be active so it will be update in the game
                    this.m_active = true;
                    // Set the health of the enemy
                    this.m_health = 10;
                    // Set the amount of damage the enemy can do
                    this.m_damage = 10;
                    // Set how fast the enemy moves
                    this.m_moveSpeed = 6f;
                    // Set the score value of the enemy
                    this.m_value = 100;

                }

                public void Update(GameTime gameTime)
                {
                    // The enemy always moves to the left so decrement it's xposition
                    this.m_position.X -= this.m_moveSpeed;
                    // Update the position of the Animation
                    this.m_enemyAnimation.position = this.m_position;
                    // Update Animation
                    this.m_enemyAnimation.Update(gameTime);
                    // If the enemy is past the screen or its health reaches 0 then deactivateit
                    if (this.m_position.X < -Width || this.m_health <= 0)
                    {
                        // By setting the Active flag to false, the game will remove this objet fromthe 
                        // active game list
                        this.m_active = false;
                    }

                }

                public void Draw(SpriteBatch spriteBatch)
                {
                    // Draw the animation
                    this.m_enemyAnimation.Draw(spriteBatch);
                }

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

    }
}
