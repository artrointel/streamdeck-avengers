﻿using System.Drawing;
using System.Collections;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using ArtrointelPlugin.Utils;


namespace ArtrointelPlugin.SDGraphics.Renderer.AnimatedEffects
{
    public class BorderWaveRenderer : CanvasRendererBase, IAnimatableRenderer
    {
        // constants

        // input data
        private readonly double mDelayInSecond;
        private readonly double mInputDurationInSecond; // animation in second
        private readonly Color mInputColor; // color of wave
        private readonly int mWaveCount;
        private readonly int mWaveThickness;
        private readonly int mWaveSpeed;
        private readonly bool mClockwise = true;

        // for internal logic
        private ImageAttributes mAlphaScaler;
        private ImageAttributes mAlphaVanishScaler;
        private double mAnimatorInterval = ValueAnimator.INTERVAL_60_PER_SEC;
        private ValueAnimator mSpinnerAnimator;
        private Brush mWaveBrush;
        private ArrayList mSpinners;
        private bool mVanishing;

        private DelayedTask mDelayedTask;

        internal class BorderSpinner // border spinner
        {
            public Rectangle mRectangle = new Rectangle();
            private bool mClockwise;
            private int mEndPoint;
            private int mSpinnerSize;
            private int mMovement;
            internal enum State
            {
                TOP = 0,
                RIGHT,
                BOTTOM,
                LEFT
            };
            State mState;

            public BorderSpinner(int spinnerSize, bool clockwise = true, State state = State.TOP)
            {
                mState = state;
                mSpinnerSize = spinnerSize;
                mEndPoint = SDCanvas.DEFAULT_IMAGE_SIZE - mSpinnerSize;
                switch (state)
                {
                    case State.TOP:
                        mRectangle.X = mRectangle.Y = 0;
                        break;
                    case State.RIGHT:
                        mRectangle.X = mEndPoint;
                        mRectangle.Y = 0;
                        break;
                    case State.BOTTOM:
                        mRectangle.X = mRectangle.Y = mEndPoint;
                        break;
                    case State.LEFT:
                        mRectangle.X = 0;
                        mRectangle.Y = mEndPoint;
                        break;

                }
                mRectangle.Width = mSpinnerSize;
                mRectangle.Height = mSpinnerSize;

                mClockwise = clockwise;
                if (mClockwise)
                    mMovement = 1;
                else
                    mMovement = -1;
            }

            public void moveNext(int inPixel = 1)
            {
                for(int i = 0; i < inPixel; i++)
                {
                    nextPositionByState();
                    if (detectCollision())
                        nextState();
                }
            }

            private bool detectCollision()
            {
                if (mRectangle.X == mEndPoint && mRectangle.Y == 0)
                {
                    return true; // TOP RIGHT
                }
                else if (mRectangle.X == mEndPoint && mRectangle.Y == mEndPoint)
                {
                    return true; // BOTTOM RIGHT
                }
                else if (mRectangle.X == 0 && mRectangle.Y == mEndPoint)
                {
                    return true; // BOTTOM LEFT
                }
                else if (mRectangle.X == 0 && mRectangle.Y == 0)
                {
                    return true; // TOP LEFT
                }
                return false;
            }

            private void nextState()
            {
                if (mClockwise)
                    ++mState;
                else
                    --mState;

                mState = (State)(((int)mState) % 4);
            }

            private void nextPositionByState()
            {
                if (mState.Equals(State.TOP))
                    mRectangle.X += mMovement;
                else if (mState.Equals(State.RIGHT))
                    mRectangle.Y += mMovement;
                else if (mState.Equals(State.BOTTOM))
                    mRectangle.X -= mMovement;
                else if (mState.Equals(State.LEFT))
                    mRectangle.Y -= mMovement;
            }
        }

        public BorderWaveRenderer(Color color, double delayInSecond, double durationInSecond, 
            int thickness = 14, double trailReducer = 0.02, int speed = 3, bool wave4 = true)
            : base(new SDCanvas.CreateInfo() { CompositingMode = CompositingMode.SourceCopy })
        {
            mInputColor = color;
            mDelayInSecond = delayInSecond;
            mInputDurationInSecond = durationInSecond;
            mWaveThickness = thickness;
            mWaveSpeed = speed;
            mSpinners = new ArrayList();
            if (wave4)
            {
                mWaveCount = 4;
                for (int i = 0; i < mWaveCount; i++)
                    mSpinners.Add(new BorderSpinner(mWaveThickness, mClockwise, (BorderSpinner.State)i));
            }
            else
            {
                mWaveCount = 2;
                mSpinners.Add(new BorderSpinner(mWaveThickness, mClockwise, BorderSpinner.State.TOP));
                mSpinners.Add(new BorderSpinner(mWaveThickness, mClockwise, BorderSpinner.State.BOTTOM));
            }

            // TODO
            // mSpinnerImage = FileIOManager.ResizeImage(FileIOManager.LoadSpinner(), thickness, thickness);
            // mSpinnerCanvas = SDCanvas.CreateCanvas();

            mAlphaScaler = new ImageAttributes();
            var cm = new ColorMatrix();
            cm.Matrix33 = (float)(1 - trailReducer); // reduce trails by alpha
            mAlphaScaler.SetColorMatrix(cm);
            mAlphaVanishScaler = new ImageAttributes();
            var vcm = new ColorMatrix();
            vcm.Matrix33 = 0.5f;
            mAlphaVanishScaler.SetColorMatrix(vcm);
            

            int animationDuration = (int)(mInputDurationInSecond * 1000.0);
            mWaveBrush = new SolidBrush(mInputColor);

            mSpinnerAnimator = new ValueAnimator(0, 1, animationDuration, mAnimatorInterval);
            mSpinnerAnimator.setAnimationListeners((value, duration) =>
            {
                double remainedDuration = animationDuration - duration;
                if(remainedDuration < 500)
                {
                    mVanishing = true;
                } else
                {
                    mVanishing = false;
                }
                
                foreach (BorderSpinner spinner in mSpinners)
                {
                    spinner.moveNext(mWaveSpeed);
                }
                invalidate();
                setVisible(true);
            }, ()=> { setVisible(false); });
        }

        public override void onRender(Graphics graphics)
        {
            if(mVanishing)
            {
                graphics.DrawImage(mOffscreenCanvas.getImage(),
                       new Rectangle(0, 0, SDCanvas.DEFAULT_IMAGE_SIZE, SDCanvas.DEFAULT_IMAGE_SIZE),
                       0, 0, SDCanvas.DEFAULT_IMAGE_SIZE, SDCanvas.DEFAULT_IMAGE_SIZE,
                       GraphicsUnit.Pixel, mAlphaVanishScaler);
            } else
            {
                graphics.DrawImage(mOffscreenCanvas.getImage(),
                       new Rectangle(0, 0, SDCanvas.DEFAULT_IMAGE_SIZE, SDCanvas.DEFAULT_IMAGE_SIZE),
                       0, 0, SDCanvas.DEFAULT_IMAGE_SIZE, SDCanvas.DEFAULT_IMAGE_SIZE,
                       GraphicsUnit.Pixel, mAlphaScaler); 
                
                RectangleF[] geometries = new RectangleF[mSpinners.Count];
                for (int i = 0; i < mSpinners.Count; i++)
                    geometries[i] = ((BorderSpinner)mSpinners[i]).mRectangle;

                graphics.FillRectangles(mWaveBrush, geometries);
            }
            base.onRender(graphics);
        }

        public void animate(bool restart)
        {
            if (mDelayInSecond > 0)
            {
                if (mDelayedTask != null)
                {
                    mDelayedTask.cancel();
                }
                mDelayedTask = new DelayedTask((int)(mDelayInSecond * 1000), () =>
                {
                    mSpinnerAnimator.start(restart);
                });
            }
            else
            {
                mSpinnerAnimator.start(restart);
            }
        }

        public void pause()
        {
            mSpinnerAnimator.pause();
        }

        public override void onDestroy()
        {
            if (mDelayedTask != null)
            {
                mDelayedTask.cancel();
            }
            mWaveBrush.Dispose();
            mSpinnerAnimator.destroy();
            base.onDestroy();
        }
    }
}
