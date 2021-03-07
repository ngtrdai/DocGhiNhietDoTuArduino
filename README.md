<H1> ĐỌC GHI DỮ LIỆU MÔI TRƯỜNG
  
---
  
<img src="https://i.imgur.com/r5aqTPZ.png" alt="Logo" title="Logo phần mềm" width="300" height="300" />

# Mục lục:
1. [Thông tin môn học](#thông-tin-môn-học)
2. [Thông tin đề tài](#thông-tin-về-đề-tài)
   1. [Sử dụng linh kiện](#linh-kiện-sử-dụng)
   2. [Sơ đồ nguyên lí](#sơ-đồ-nguyên-lí)
   3. [Sơ đồ PCB](#sơ-đồ-pcb)
   4. [Sản phẩm thực tế](#sản-phẩm-thực-tế)
3. [Giao tiếp Arduino với các cảm biến](#giao-tiếp-giữa-arduino-và-cảm-biến)
4. [Giới thiệu về phần mềm](#giới-thiệu-về-phần-mềm)
   1. [Chức năng chính](#những-chức-năng-chính)
   2. [Nhận dữ liệu từ Arduino và xử lý dữ liệu bằng C#](#nhận-dữ-liệu-từ-arduino-và-xử-lý-dữ-liệu-bằng-c)
   3. [Giải pháp ghi dữ liệu vào Excel](#giải-pháp-ghi-dữ-liệu-vào-excel)
   4. [Giải pháp đồ thị](#giải-pháp-về-lưu-đồ-thị)
5. [Lời kết](#-lời-kết)
# Thông tin môn học:
  
* **Môn học:** Cảm biến và cơ cấu chấp hành.
* **Giảng viên:** Dương Thế Phong.
* **Thành viên:**
```
Nguyễn Trọng Đại - 19146146 - @ngtrdai.
```
```
Trần Triệu Vĩ - 19146301 - @trantrieuvi.
```


# Thông tin về đề tài:
---
## Sử dụng linh kiện

## Sơ đồ nguyên lí

## Sơ đồ PCB

## Sản phẩm thực tế

# Giao tiếp giữa Arduino và cảm biến
---

```
    #include <Wire.h> 
    #include <LiquidCrystal_I2C.h>
    #include <dht_nonblocking.h>
    #include "MQ135.h"
    LiquidCrystal_I2C lcd(0x27,16,2); 
    static const int DHT_SENSOR_PIN = A0;
    #define DHT_SENSOR_TYPE DHT_TYPE_22
    DHT_nonblocking dht_sensor(DHT_SENSOR_PIN, DHT_SENSOR_TYPE);
    #define PIN_MQ135 A3
    MQ135 mq135_sensor = MQ135(PIN_MQ135);

    void setup(){
        Serial.begin(9600);
        lcd.init();                    
        lcd.backlight();
        lcd.setCursor(4,0);
        lcd.print("CB - CCCH");
        lcd.setCursor(0,1);
        lcd.print("@ngtrdai-@trtrvi");
    }

    static bool doThongSoMoiTruong(float* NhietDo, float* DoAm){
        static unsigned long mocThoiGian = millis();
        if(millis() - mocThoiGian > 1000ul){
            if(dht_sensor.measure(NhietDo, DoAm) == true){
            mocThoiGian = millis();
            return true;
            }
        }
        return false;
    }

    void loop(){
        float doAm;
        float nhietDo;
        float khongKhi;
        int doAmDat = analogRead(A1);
        doAmDat = map(doAmDat,0, 1023,0, 100);
        if(doThongSoMoiTruong(&nhietDo, &doAm) == true){
            khongKhi = mq135_sensor.getCorrectedPPM(nhietDo, doAm);
            Serial.print(nhietDo);
            Serial.print(",");
            Serial.print(doAm);
            Serial.print(",");
            Serial.print(khongKhi);
            Serial.print(",");
            Serial.println(doAmDat);
            lcd.setCursor(0,0);
            lcd.print("NHIET DO:");
            lcd.setCursor(9,0);
            lcd.print(nhietDo);
            lcd.setCursor(0,1);
            lcd.print("DO AM:");
            lcd.setCursor(6,1);
            lcd.print(doAm);
        }
    }
```

# Giới thiệu về phần mềm
---

## Những chức năng chính

## Nhận dữ liệu từ Arduino và xử lý dữ liệu bằng C#

## Giải pháp ghi dữ liệu vào Excel

## Giải pháp về lưu đồ thị

# Lời kết
---
