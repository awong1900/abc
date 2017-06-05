﻿#define SIMULATE
using Microsoft.Azure.Devices.Client;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
#if SIMULATE
#else
using GrovePi;
using GrovePi.I2CDevices;
using GrovePi.Sensors;
#endif

namespace AzureGroveKit
{
    class SensorController
    {
#if SIMULATE
        public GroveMessage GetSensorValue()
        {
            GroveMessage message = new GroveMessage();
            message.Temp = "25";
            message.Hum = "69";
            message.Sound = "23";
            message.Light = "112";
            message.GasSO = 23;
            message.PIR = "True";
            message.Timestamp = DateTime.Now.ToString();

            return message;
        }

        public void DisplayLCD(String msg)
        {
            Debug.WriteLine("Display: " + msg);
        }

        public void ControlMotoDriver(Boolean onoff)
        {
            Debug.WriteLine("\t turn " + onoff);
        }
#else

        IDHTTemperatureAndHumiditySensor temphumiSensor;
        ISoundSensor soundSensor;
        ILightSensor lightSensor;
        IGasSensorMQ2 gasSensor;
        IPIRMotionSensor pirMotion;
        IRgbLcdDisplay display;
        IMiniMotorDriver motor;

        public SensorController() {
            temphumiSensor = DeviceFactory.Build.DHTTemperatureAndHumiditySensor(Pin.DigitalPin2, DHTModel.Dht11);
            soundSensor = DeviceFactory.Build.SoundSensor(Pin.AnalogPin0);
            lightSensor = DeviceFactory.Build.LightSensor(Pin.AnalogPin1);
            gasSensor = DeviceFactory.Build.GasSensorMQ2(Pin.AnalogPin2);
            pirMotion = DeviceFactory.Build.PIRMotionSensor(Pin.DigitalPin3);
            display = DeviceFactory.Build.RgbLcdDisplay();
            // Mini Motor Driver refer to: http://wiki.seeed.cc/Grove-Mini_I2C_Motor_Driver_v1.0/#change-default-i2c-address
            motor = DeviceFactory.Build.MiniMotorDriver(0xD0, 0xC0);
        }

        public GroveMessage GetSensorValue()
        {
            GroveMessage message = new GroveMessage();

            try
            {
                temphumiSensor.Measure();
                message.Temp = temphumiSensor.TemperatureInCelsius.ToString();
                message.Hum = temphumiSensor.Humidity.ToString();
                message.Sound = soundSensor.SensorValue().ToString();
                message.Light = lightSensor.SensorValue().ToString();
                message.GasSO = gasSensor.SensorValue().ToString();
                message.PIR = pirMotion.IsPeopleDetected().ToString();
                message.Timestamp = DateTime.Now.ToString();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return message;
        }


        public void DisplayLCD(String msg)
        {
            display.SetText(msg).SetBacklightRgb(255, 50, 255);
        }

        public void ControlMotoDriver(Boolean onoff)
        {
            if (onoff)
            {
                motor.drive1(100);
                motor.drive2(100);
            } else
            {
                motor.drive1(0);
                motor.drive2(0);
            }
        }
#endif
    }

}