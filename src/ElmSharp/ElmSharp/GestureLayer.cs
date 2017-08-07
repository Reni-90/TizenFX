/*
 * Copyright (c) 2016 Samsung Electronics Co., Ltd All Rights Reserved
 *
 * Licensed under the Apache License, Version 2.0 (the License);
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an AS IS BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace ElmSharp
{
    /// <summary>
    /// The GestureLayer is used to detect gestures.
    /// Inherits Widget
    /// </summary>
    public class GestureLayer : Widget
    {
        private readonly Interop.Elementary.GestureEventCallback _gestureCallback;

        // Important: don't remove items from _handlers list
        // The list can grow up to (number of GestureType) * (number of GestureState)
        // but all gestures share the callback and you don't want to desynchronize mapping
        private readonly List<NativeCallback> _handlers = new List<NativeCallback>();

        /// <summary>
        /// Creates and initializes a new instance of GestureLayer class.
        /// </summary>
        /// <param name="parent">The parent is a given container which will be attached by GestureLayer as a child. It's <see cref="EvasObject"/> type.</param>
        public GestureLayer(EvasObject parent) : base(parent)
        {
            _gestureCallback = new Interop.Elementary.GestureEventCallback(GestureCallbackHandler);
        }

        /// <summary>
        /// Enumeration for supported gesture types.
        /// </summary>
        public enum GestureType
        {
            /// <summary>
            /// N fingers single taps
            /// </summary>
            Tap = 1,

            /// <summary>
            /// N fingers single long-taps
            /// </summary>
            LongTap,

            /// <summary>
            /// N fingers double-single taps
            /// </summary>
            DoubleTap,

            /// <summary>
            /// N fingers triple-single taps
            /// </summary>
            TripleTap,

            /// <summary>
            /// Reports momentum in the direction of move
            /// </summary>
            Momentum,

            /// <summary>
            /// N fingers line gesture
            /// </summary>
            Line,

            /// <summary>
            /// N fingers flick gesture
            /// </summary>
            Flick,

            /// <summary>
            /// Zoom
            /// </summary>
            Zoom,

            /// <summary>
            /// Rotate
            /// </summary>
            Rotate,
        }

        /// <summary>
        /// Enumeration for gesture states.
        /// </summary>
        public enum GestureState
        {
            /// <summary>
            /// Gesture not started
            /// </summary>
            Undefined = -1,

            /// <summary>
            /// Gesture started
            /// </summary>
            Start,

            /// <summary>
            /// Gesture is ongoing
            /// </summary>
            Move,

            /// <summary>
            /// Gesture completed
            /// </summary>
            End,

            /// <summary>
            /// Ongoing gesture is aborted
            /// </summary>
            Abort,
        }

        #region Properties

        /// <summary>
        /// Sets or gets the repeat-events setting.
        /// </summary>
        public bool HoldEvents
        {
            get
            {
                return Interop.Elementary.elm_gesture_layer_hold_events_get(Handle);
            }
            set
            {
                Interop.Elementary.elm_gesture_layer_hold_events_set(Handle, value);
            }
        }

        /// <summary>
        /// Sets or gets the gesture layer continues enable of an object
        /// </summary>
        public bool Continues
        {
            get
            {
                return Interop.Elementary.elm_gesture_layer_continues_enable_get(Handle);
            }
            set
            {
                Interop.Elementary.elm_gesture_layer_continues_enable_set(Handle, value);
            }
        }

        /// <summary>
        /// Sets or gets the gesture layer finger-size for taps.
        /// </summary>
        public int TapFingerSize
        {
            get
            {
                return Interop.Elementary.elm_gesture_layer_tap_finger_size_get(Handle);
            }
            set
            {
                Interop.Elementary.elm_gesture_layer_tap_finger_size_set(Handle, value);
            }
        }

        /// <summary>
        /// Sets or gets the gesture layer long tap start timeout of an object
        /// </summary>
        public double LongTapTimeout
        {
            get
            {
                return Interop.Elementary.elm_gesture_layer_long_tap_start_timeout_get(Handle);
            }
            set
            {
                Interop.Elementary.elm_gesture_layer_long_tap_start_timeout_set(Handle, value);
            }
        }

        /// <summary>
        /// Sets or gets the gesture layer double tap timeout of an object
        /// </summary>
        public double DoubleTapTimeout
        {
            get
            {
                return Interop.Elementary.elm_gesture_layer_double_tap_timeout_get(Handle);
            }
            set
            {
                Interop.Elementary.elm_gesture_layer_double_tap_timeout_set(Handle, value);
            }
        }

        /// <summary>
        /// Sets or gets the gesture layer flick time limit (in ms) of an object
        /// </summary>
        public int FlickTimeLimit
        {
            get
            {
                return (int)Interop.Elementary.elm_gesture_layer_flick_time_limit_ms_get(Handle);
            }
            set
            {
                Interop.Elementary.elm_gesture_layer_flick_time_limit_ms_set(Handle, (UInt32)value);
            }
        }

        /// <summary>
        /// Sets or gets the gesture layer line min length of an object
        /// </summary>
        public int MinimumLineLength
        {
            get
            {
                return Interop.Elementary.elm_gesture_layer_line_min_length_get(Handle);
            }
            set
            {
                Interop.Elementary.elm_gesture_layer_line_min_length_set(Handle, value);
            }
        }

        /// <summary>
        /// Sets or gets  the gesture layer line angular tolerance of an object
        /// </summary>
        public double LineAngularTolerance
        {
            get
            {
                return Interop.Elementary.elm_gesture_layer_line_angular_tolerance_get(Handle);
            }
            set
            {
                Interop.Elementary.elm_gesture_layer_line_angular_tolerance_set(Handle, value);
            }
        }

        /// <summary>
        /// Sets or gets the gesture layer line distance tolerance of an object
        /// </summary>
        public int LineDistanceTolerance
        {
            get
            {
                return Interop.Elementary.elm_gesture_layer_line_distance_tolerance_get(Handle);
            }
            set
            {
                Interop.Elementary.elm_gesture_layer_line_distance_tolerance_set(Handle, value);
            }
        }

        /// <summary>
        /// Sets or gets step-value for rotate action.
        /// </summary>
        public double RotateStep
        {
            get
            {
                return Interop.Elementary.elm_gesture_layer_rotate_step_get(Handle);
            }
            set
            {
                Interop.Elementary.elm_gesture_layer_rotate_step_set(Handle, value);
            }
        }

        /// <summary>
        /// Sets or gets the gesture layer rotate angular tolerance of an object
        /// </summary>
        public double RotateAngularTolerance
        {
            get
            {
                return Interop.Elementary.elm_gesture_layer_rotate_angular_tolerance_get(Handle);
            }
            set
            {
                Interop.Elementary.elm_gesture_layer_rotate_angular_tolerance_set(Handle, value);
            }
        }

        /// <summary>
        /// Sets or gets control step value for zoom action.
        /// </summary>
        public double ZoomStep
        {
            get
            {
                return Interop.Elementary.elm_gesture_layer_zoom_step_get(Handle);
            }
            set
            {
                Interop.Elementary.elm_gesture_layer_zoom_step_set(Handle, value);
            }
        }

        /// <summary>
        /// Sets or gets the gesture layer zoom distance tolerance of an object
        /// </summary>
        public int ZoomDistanceTolerance
        {
            get
            {
                return Interop.Elementary.elm_gesture_layer_zoom_distance_tolerance_get(Handle);
            }
            set
            {
                Interop.Elementary.elm_gesture_layer_zoom_distance_tolerance_set(Handle, value);
            }
        }

        /// <summary>
        /// Sets or gets the gesture layer zoom finger factor of an object
        /// </summary>
        public double ZoomFingerFactor
        {
            get
            {
                return Interop.Elementary.elm_gesture_layer_zoom_finger_factor_get(Handle);
            }
            set
            {
                Interop.Elementary.elm_gesture_layer_zoom_finger_factor_set(Handle, value);
            }
        }

        /// <summary>
        /// Sets or gets the gesture layer zoom wheel factor of an object
        /// </summary>
        public double ZoomWheelFactor
        {
            get
            {
                return Interop.Elementary.elm_gesture_layer_zoom_wheel_factor_get(Handle);
            }
            set
            {
                Interop.Elementary.elm_gesture_layer_zoom_wheel_factor_set(Handle, value);
            }
        }

        #endregion Properties

        /// <summary>
        /// Attach a gesture layer widget to an Evas object (setting the widget's target).
        /// A gesture layer's target may be any Evas object. This object will be used to listen to mouse and key events.
        /// </summary>
        /// <param name="target">The object to attach.</param>
        public void Attach(EvasObject target)
        {
            Interop.Elementary.elm_gesture_layer_attach(Handle, target.Handle);
        }

        /// <summary>
        /// Set the gesture state change callback.
        /// When all callbacks for the gesture are set to null, it means this gesture is disabled.
        /// </summary>
        /// <param name="type">The gesture you want to track state of.</param>
        /// <param name="state">The event the callback tracks (START, MOVE, END, ABORT).</param>
        /// <param name="action">The callback itself.</param>
        public void SetGestureCallback(GestureType type, GestureState state, Action<object> action)
        {
            lock (_handlers)
            {
                bool found = false;
                int i = 0;
                // if this (type, state) already exists in _handlers, we will reuse it
                foreach (var handler in _handlers)
                {
                    if (handler.Type == type && handler.State == state)
                    {
                        found = true;
                        break;
                    }
                    i++;
                }
                if (found)
                {
                    // if we are changing null -> not-null, or not-null -> null, then inform the EFL
                    if (_handlers[i].Action == null ^ action == null)
                        Interop.Elementary.elm_gesture_layer_cb_set(Handle, type, state, action == null ? null : _gestureCallback, new IntPtr(i));
                    // overwrite previous action
                    _handlers[i].Action = action;
                }
                else
                {
                    if (action == null)
                    {
                        // ignore unsetting a handler for event which was not registered yet?
                        return;
                    }
                    // (type, state) was not found, so we are adding a new entry and registering the callback
                    _handlers.Add(new NativeCallback(type, state, action));
                    // callback is always the same, the event is recognised by the index in _handler list (the index is passed as data)
                    Interop.Elementary.elm_gesture_layer_cb_set(Handle, type, state, _gestureCallback, new IntPtr(i));
                }
            }
        }

        /// <summary>
        /// clear the gesture state change callback.
        /// </summary>
        public void ClearCallbacks()
        {
            lock (_handlers)
            {
                int i = 0;
                foreach (var handler in _handlers)
                {
                    if (handler.Action != null)
                    {
                        Interop.Elementary.elm_gesture_layer_cb_set(Handle, handler.Type, handler.State, null, new IntPtr(i));
                        handler.Action = null;
                    }
                    i++;
                }
            }
        }

        #region Typed callback setting methods

        // Following methods have been added for convenience, so the user will not have to convert Info structures himself
        /// <summary>
        /// Set the tap callback.
        /// </summary>
        /// <param name="type">The gesture you want to track state of.</param>
        /// <param name="state">The event the callback tracks (START, MOVE, END, ABORT).</param>
        /// <param name="action">The callback itself.</param>
        public void SetTapCallback(GestureType type, GestureState state, Action<TapData> action)
        {
            SetCallback(type, state, action);
        }

        /// <summary>
        /// Set the gesture state change callback with Momentum Gesture Type
        /// </summary>
        /// <param name="state">The event the callback tracks (START, MOVE, END, ABORT).</param>
        /// <param name="action">The callback itself.</param>
        public void SetMomentumCallback(GestureState state, Action<MomentumData> action)
        {
            SetCallback(GestureType.Momentum, state, action);
        }

        /// <summary>
        /// Set the gesture state change callback with Line Gesture Type
        /// </summary>
        /// <param name="state">The event the callback tracks (START, MOVE, END, ABORT).</param>
        /// <param name="action">The callback itself.</param>
        public void SetLineCallback(GestureState state, Action<LineData> action)
        {
            SetCallback(GestureType.Line, state, action);
        }

        /// <summary>
        /// Set the gesture state change callback with Flick Gesture Type
        /// </summary>
        /// <param name="state">The event the callback tracks (START, MOVE, END, ABORT).</param>
        /// <param name="action">The callback itself.</param>
        public void SetFlickCallback(GestureState state, Action<LineData> action)
        {
            SetCallback(GestureType.Flick, state, action);
        }

        /// <summary>
        /// Set the gesture state change callback with Zoom Gesture Type
        /// </summary>
        /// <param name="state">The event the callback tracks (START, MOVE, END, ABORT).</param>
        /// <param name="action">The callback itself.</param>
        public void SetZoomCallback(GestureState state, Action<ZoomData> action)
        {
            SetCallback(GestureType.Zoom, state, action);
        }

        /// <summary>
        /// Set the gesture state change callback with Rotate Gesture Type
        /// </summary>
        /// <param name="state">The event the callback tracks (START, MOVE, END, ABORT).</param>
        /// <param name="action">The callback itself.</param>
        public void SetRotateCallback(GestureState state, Action<RotateData> action)
        {
            SetCallback(GestureType.Rotate, state, action);
        }

        #endregion Typed callback setting methods

        /// <summary>
        /// Call this function to construct a new gesture-layer object.
        /// </summary>
        /// <param name="parent">The gesture layer's parent widget.</param>
        /// <returns></returns>
        protected override IntPtr CreateHandle(EvasObject parent)
        {
            return Interop.Elementary.elm_gesture_layer_add(parent);
        }

        /// <summary>
        /// clear the gesture state change callback.
        /// </summary>
        protected override void OnUnrealize()
        {
            ClearCallbacks();
            base.OnUnrealize();
        }

        private void SetCallback<T>(GestureType type, GestureState state, Action<T> action)
        {
            if (action == null)
                SetGestureCallback(type, state, null);
            else
                SetGestureCallback(type, state, new Action<object>((info) => action((T)info)));
        }

        private void GestureCallbackHandler(IntPtr data, IntPtr event_info)
        {
            // so EFL called our callback, lets use data to find the right Action to call
            var handlerIndex = (int)data;
            // thanks to the fact that we never remove item from _handlers, we don't need a lock here
            if (handlerIndex < 0 || handlerIndex >= _handlers.Count)
                return;
            Action<object> action = _handlers[handlerIndex].Action;
            if (action == null)
                return;
            // the interpretation of the event_info struct pointer depends on the GestureType
            switch (_handlers[handlerIndex].Type)
            {
                case GestureType.Tap:
                case GestureType.LongTap:
                case GestureType.DoubleTap:
                case GestureType.TripleTap:
                    action(Marshal.PtrToStructure<TapData>(event_info));
                    break;

                case GestureType.Momentum:
                    action(Marshal.PtrToStructure<MomentumData>(event_info));
                    break;

                case GestureType.Line:
                case GestureType.Flick:
                    action(Marshal.PtrToStructure<LineData>(event_info));
                    break;

                case GestureType.Zoom:
                    action(Marshal.PtrToStructure<ZoomData>(event_info));
                    break;

                case GestureType.Rotate:
                    action(Marshal.PtrToStructure<RotateData>(event_info));
                    break;
            }
        }

        #region Info structures

        /// <summary>
        /// The struct of TapData
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct TapData
        {
            /// <summary>
            /// The x coordinate of the center point.
            /// </summary>
            public Int32 X;

            /// <summary>
            /// The y coordinate of the center point.
            /// </summary>
            public Int32 Y;

#pragma warning disable 3003

            /// <summary>
            /// The number of fingers tapped.
            /// </summary>
            public UInt32 FingersCount;

            /// <summary>
            /// The timestamp.
            /// </summary>
            public UInt32 Timestamp;

#pragma warning restore 3003
        }

        /// <summary>
        /// The struct of MomentumData
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MomentumData
        {
            /// <summary>
            /// Final-swipe direction starting point on X.
            /// </summary>
            public Int32 X1;

            /// <summary>
            /// Final-swipe direction starting point on Y.
            /// </summary>
            public Int32 Y1;

            /// <summary>
            /// Final-swipe direction ending point on X.
            /// </summary>
            public Int32 X2;

            /// <summary>
            /// Final-swipe direction ending point on Y
            /// </summary>
            public Int32 Y2;

#pragma warning disable 3003

            /// <summary>
            /// Timestamp of start of final x-swipe.
            /// </summary>
            public UInt32 HorizontalSwipeTimestamp;

            /// <summary>
            /// Timestamp of start of final y-swipe.
            /// </summary>
            public UInt32 VerticalSwipeTimestamp;

            /// <summary>
            /// Momentum on X.
            /// </summary>
            public Int32 HorizontalMomentum;

            /// <summary>
            /// Momentum on Y.
            /// </summary>
            public Int32 VerticalMomentum;

            /// <summary>
            /// Number of fingers.
            /// </summary>
            public UInt32 FingersCount;

#pragma warning restore 3003
        }

        /// <summary>
        /// The struct of LineData
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct LineData
        {
            /// <summary>
            /// Final-swipe direction starting point on X.
            /// </summary>
            public Int32 X1;

            /// <summary>
            /// Final-swipe direction starting point on Y.
            /// </summary>
            public Int32 Y1;

            /// <summary>
            /// Final-swipe direction ending point on X.
            /// </summary>
            public Int32 X2;

            /// <summary>
            /// Final-swipe direction ending point on Y
            /// </summary>
            public Int32 Y2;

#pragma warning disable 3003

            /// <summary>
            /// Timestamp of start of final x-swipe.
            /// </summary>
            public UInt32 HorizontalSwipeTimestamp;

            /// <summary>
            /// Timestamp of start of final y-swipe.
            /// </summary>
            public UInt32 VerticalSwipeTimestamp;

            /// <summary>
            /// Momentum on X.
            /// </summary>
            public Int32 HorizontalMomentum;

            /// <summary>
            /// Momentum on Y.
            /// </summary>
            public Int32 VerticalMomentum;

            /// <summary>
            /// Number of fingers.
            /// </summary>
            public UInt32 FingersCount;

#pragma warning restore 3003

            /// <summary>
            /// Angle (direction) of lines.
            /// </summary>
            public double Angle;
        }

        /// <summary>
        /// The struct of ZoomData
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ZoomData
        {
            /// <summary>
            /// The x coordinate of zoom center point reported to user.
            /// </summary>
            public Int32 X;

            /// <summary>
            /// The y coordinate of zoom center point reported to user.
            /// </summary>
            public Int32 Y;

            /// <summary>
            /// The radius (distance) between fingers reported to user.
            /// </summary>
            public Int32 Radius;

            /// <summary>
            /// The zoom value. 1.0 means no zoom.
            /// </summary>
            public double Zoom;

            /// <summary>
            /// Zoom momentum: zoom growth per second (NOT YET SUPPORTED).
            /// </summary>
            private double Momentum;
        }

        /// <summary>
        /// The struct of RotateData
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RotateData
        {
            /// <summary>
            /// The x coordinate of rotation center point reported to user.
            /// </summary>
            public Int32 X;

            /// <summary>
            /// The y coordinate of rotation center point reported to user.
            /// </summary>
            public Int32 Y;

            /// <summary>
            /// The radius (distance) between fingers reported to user.
            /// </summary>
            public Int32 Radius;

            /// <summary>
            /// The start-angle.
            /// </summary>
            public double BaseAngle;

            /// <summary>
            /// The rotation value. 0.0 means no rotation.
            /// </summary>
            public double Angle;

            /// <summary>
            /// Rotation momentum: rotation done per second (NOT YET SUPPORTED).
            /// </summary>
            private double Momentum;
        }

        #endregion Info structures

        /// <summary>
        /// Config is a static class, it provides gestureLayer's timeout information.
        /// </summary>
        public static class Config
        {
            /// <summary>
            /// Sets or gets the duration for occurring long tap event of gesture layer.
            /// </summary>
            public static double DefaultLongTapTimeout
            {
                get
                {
                    return Interop.Elementary.elm_config_glayer_long_tap_start_timeout_get();
                }
                set
                {
                    Interop.Elementary.elm_config_glayer_long_tap_start_timeout_set(value);
                }
            }

            /// <summary>
            /// Sets or gets the duration for occurring double tap event of gesture layer.
            /// </summary>
            public static double DefaultDoubleTapTimeout
            {
                get
                {
                    return Interop.Elementary.elm_config_glayer_double_tap_timeout_get();
                }
                set
                {
                    Interop.Elementary.elm_config_glayer_double_tap_timeout_set(value);
                }
            }
        }

        private class NativeCallback
        {
            public readonly GestureType Type;
            public readonly GestureState State;
            public Action<object> Action;

            public NativeCallback(GestureType type, GestureState state, Action<object> action)
            {
                Type = type;
                State = state;
                Action = action;
            }
        }
    }
}