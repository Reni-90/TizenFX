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

namespace Tizen.Sensor
{
    /// <summary>
    /// The GyroscopeRotationVectorSensor class is used for registering callbacks for the gyroscope rotation vector sensor and getting the gyroscope rotation vector data.
    /// </summary>
    /// <since_tizen> 3 </since_tizen>
    public sealed class GyroscopeRotationVectorSensor : Sensor
    {
        private const string GyroscopeRVKey = "http://tizen.org/feature/sensor.gyroscope_rotation_vector";

        /// <summary>
        /// Gets the X component of the gyroscope rotation vector.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        /// <value> X </value>
        public float X { get; private set; } = float.MinValue;

        /// <summary>
        /// Gets the Y component of the gyroscope rotation vector.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        /// <value> Y </value>
        public float Y { get; private set; } = float.MinValue;

        /// <summary>
        /// Gets the Z component of the gyroscope rotation vector.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        /// <value> Z </value>
        public float Z { get; private set; } = float.MinValue;

        /// <summary>
        /// Gets the W component of the gyroscope rotation vector.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        /// <value> W </value>
        public float W { get; private set; } = float.MinValue;

        /// <summary>
        /// Gets the accuracy of the gyroscope rotation vector data.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        /// <value> Accuracy </value>
        public SensorDataAccuracy Accuracy { get; private set; }

        /// <summary>
        /// Returns true or false based on whether the gyroscope rotation vector sensor is supported by the device.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        /// <value><c>true</c> if supported; otherwise <c>false</c>.</value>
        public static bool IsSupported
        {
            get
            {
                Log.Info(Globals.LogTag, "Checking if the GyroscopeRotationVectorSensor is supported");
                return CheckIfSupported(SensorType.GyroscopeRotationVectorSensor, GyroscopeRVKey);
            }
        }

        /// <summary>
        /// Returns the number of the gyroscope rotation vector sensors available on the device.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        /// <value> The count of accelerometer rotation vector sensors. </value>
        public static int Count
        {
            get
            {
                Log.Info(Globals.LogTag, "Getting the count of gyroscope rotation vector sensors");
                return GetCount();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tizen.Sensor.GyroscopeRotationVectorSensor"/> class.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        /// <feature>http://tizen.org/feature/sensor.gyroscope_rotation_vector</feature>
        /// <exception cref="ArgumentException">Thrown when an invalid argument is used.</exception>
        /// <exception cref="NotSupportedException">Thrown when the sensor is not supported.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the operation is invalid for the current state.</exception>
        /// <param name='index'>
        /// Index. Default value for this is 0. Index refers to a particular gyroscope rotation vector sensor in case of multiple sensors.
        /// </param>
        public GyroscopeRotationVectorSensor(uint index = 0) : base(index)
        {
            Log.Info(Globals.LogTag, "Creating GyroscopeRotationVectorSensor object");
        }

        internal override SensorType GetSensorType()
        {
            return SensorType.GyroscopeRotationVectorSensor;
        }

        /// <summary>
        /// An event handler for storing the callback functions for the event corresponding to the change in the gyroscope rotation vector sensor data.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>

        public event EventHandler<GyroscopeRotationVectorSensorDataUpdatedEventArgs> DataUpdated;

        private static int GetCount()
        {
            IntPtr list;
            int count;
            int error = Interop.SensorManager.GetSensorList(SensorType.GyroscopeRotationVectorSensor, out list, out count);
            if (error != (int)SensorError.None)
            {
                Log.Error(Globals.LogTag, "Error getting sensor list for gyroscope rotation vector");
                count = 0;
            }
            else
                Interop.Libc.Free(list);
            return count;
        }

        private static Interop.SensorListener.SensorEventCallback _callback;

        internal override void EventListenStart()
        {
            _callback = (IntPtr sensorHandle, IntPtr eventPtr, IntPtr data) => {
                Interop.SensorEventStruct sensorData = Interop.IntPtrToEventStruct(eventPtr);

                TimeSpan = new TimeSpan((Int64)sensorData.timestamp);
                X = sensorData.values[0];
                Y = sensorData.values[1];
                Z = sensorData.values[2];
                W = sensorData.values[3];
                Accuracy = sensorData.accuracy;

                DataUpdated?.Invoke(this, new GyroscopeRotationVectorSensorDataUpdatedEventArgs(sensorData.values, sensorData.accuracy));
            };

            int error = Interop.SensorListener.SetEventCallback(ListenerHandle, Interval, _callback, IntPtr.Zero);
            if (error != (int)SensorError.None)
            {
                Log.Error(Globals.LogTag, "Error setting event callback for gyroscope rotation vector sensor");
                throw SensorErrorFactory.CheckAndThrowException(error, "Unable to set event callback for gyroscope rotation vector");
            }
        }

        internal override void EventListenStop()
        {
            int error = Interop.SensorListener.UnsetEventCallback(ListenerHandle);
            if (error != (int)SensorError.None)
            {
                Log.Error(Globals.LogTag, "Error unsetting event callback for gyroscope rotation vector sensor");
                throw SensorErrorFactory.CheckAndThrowException(error, "Unable to unset event callback for gyroscope rotation vector");
            }
        }
    }
}
