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
   3. [Nhận dữ liệu từ Arduino và xử lý dữ liệu bằng C#](#nhận-dữ-liệu-từ-arduino-và-xử-lý-dữ-liệu-bằng-c)
   4. [Giải pháp ghi dữ liệu vào Excel](#giải-pháp-ghi-dữ-liệu-vào-excel)
   5. [Giải pháp đồ thị](#giải-pháp-về-lưu-đồ-thị)
5. [Lời kết](#lời-kết)
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
    #include <Wire.h>                                               // Khai báo thư viện Wire.h
    #include <LiquidCrystal_I2C.h>                                  // Khai báo thư viện LiquidCrystal_I2C.h để sử dụng LCD.
    #include <dht_nonblocking.h>                                    // Khai báo thư viện cảm biến DHT
    #include "MQ135.h"                                              // Khai báo thư viện cảm biến MQ135
    LiquidCrystal_I2C lcd(0x27,16,2);                               // Thiết lập LCD
    static const int DHT_SENSOR_PIN = A0;                           // Chân nhận dữ liệu Analog từ cảm biến DHT
    #define DHT_SENSOR_TYPE DHT_TYPE_22                             // Khai báo loại cảm biến DHT cụ thể là DHT22
    DHT_nonblocking dht_sensor(DHT_SENSOR_PIN, DHT_SENSOR_TYPE);    // Thiết lập DHT
    #define PIN_MQ135 A3                                            // Chân nhận dữ liệu Analog từ cảm biến MQ135
    MQ135 mq135_sensor = MQ135(PIN_MQ135);                          // Tạo đối tượng cảm biến MQ135

    void setup(){
        Serial.begin(9600);                                         // Thiết lập Baudrate cho cổng Serial
        lcd.init();                                                 // Khởi tạo LCD
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
            lcd.clear();
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
- [X] Ghi nhận dữ liệu nhiệt độ, độ ẩm, chất lượng không khí, và độ ẩm đất.
- [X] Kết nối qua cổng Serial Port, có thể sử dụng với Module Bluetooth.
- [X] Vẽ đồ thị theo thời gian thực.
- [X] Lưu đồ thị với file .png.
- [X] Hiển thị DataGrid dữ liệu ghi nhận theo thời gian thực.
- [X] Lưu dữ liệu với file .csv để đọc được với Excel.
- [ ] Cập nhật sau . . .
### Screenshot
![Ảnh chụp màn hình](https://github.com/ngtrdai/DocGhiNhietDoTuArduino/blob/master/Screenshot.png)
## Nhận dữ liệu từ Arduino và xử lý dữ liệu bằng C#
### Sự kiện khi chọn cổng COM và nhấn nút kết nối
```
  private void btnKetNoi_Click(object sender, RoutedEventArgs e){
      if (comList.Text == ""){
          MessageBox.Show("Xin vui lòng chọn cổng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
      }
      else{
          if (serialPort.IsOpen){
              serialPort.Close();
              btnKetNoi.Content = "Kết nối";
              comList.IsEnabled = true;
          }
          else{
              try{
                  serialPort.PortName = comList.Text;
                  serialPort.BaudRate = 9600;
                  serialPort.Open();
                  btnKetNoi.Content = "Ngắt kết nối";
                  comList.IsEnabled = false;
              }
              catch{
                  MessageBox.Show("Không thể mở cổng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
              }
          }
      }
  }
```
### Timer nhận dữ liệu
```
  private void TimerNhanDuLieu_Tick(object sender, EventArgs e){
      if (!serialPort.IsOpen){
          trangThaiKetNoi.Text = "Chưa kết nối";
      }
      else if (serialPort.IsOpen){
          trangThaiKetNoi.Text = "Đã kết nối";
          string duLieuNhan = serialPort.ReadLine().ToString();
          string[] arrListStr = duLieuNhan.Split(',');
          string NhietDo = arrListStr[0];
          string DoAm = arrListStr[1];
          string KhongKhi = arrListStr[2];
          string DoAmDat = arrListStr[3].Substring(0, arrListStr[3].Length - 1);
          nhietDo.Text = NhietDo;
          doAm.Text = DoAm;
          khongKhi.Text = KhongKhi;
          doAmDat.Text = DoAmDat;
          SeriesCollection[0].Values.Add(Convert.ToDouble(NhietDo));
          SeriesCollection[1].Values.Add(Convert.ToDouble(DoAm));
          SeriesCollection[2].Values.Add(Convert.ToDouble(KhongKhi));
          SeriesCollection[3].Values.Add(Convert.ToDouble(DoAmDat));
          dataGrid.Items.Add(new Data(){
              cNgay = DateTime.Now.ToShortDateString(),
              cNhietDo = Convert.ToDouble(NhietDo),
              cDoAm = Convert.ToDouble(DoAm),
              cThoiGian = DateTime.Now.ToShortTimeString(),
              cKhongKhi = Convert.ToDouble(KhongKhi),
              cDoAmDat = Convert.ToDouble(DoAmDat)});
      }
  }
```

## Giải pháp ghi dữ liệu vào Excel
```
  private void btnSaveCSV_Click(object sender, RoutedEventArgs e){
      dataGrid.SelectAllCells();
      dataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
      ApplicationCommands.Copy.Execute(null, this.dataGrid);
      dataGrid.UnselectAllCells();
      String result = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
      string fileName = "";
      SaveFileDialog saveFileDialog = new SaveFileDialog();
      saveFileDialog.Filter = "Data file (*.csv)|*.csv";
      saveFileDialog.ShowDialog();
      fileName = saveFileDialog.FileName;
      if (fileName != ""){
          StreamWriter sw = new StreamWriter(fileName, true, Encoding.GetEncoding("iso-8859-1"));
          sw.WriteLine(result);
          sw.Close();
          Process.Start(fileName);
      }
  }
```
## Giải pháp về lưu đồ thị
```
  private void btnLuuDoThi_Click(object sender, RoutedEventArgs e){
      var bitmap = new RenderTargetBitmap((int)DoThi.ActualWidth, (int)DoThi.ActualHeight, 96, 96, PixelFormats.Pbgra32);
      bitmap.Render(DoThi);
      var frame = BitmapFrame.Create(bitmap);
      var encoder = new PngBitmapEncoder();
      encoder.Frames.Add(frame);
      string fileName = "";
      SaveFileDialog saveFileDialog = new SaveFileDialog();
      saveFileDialog.Filter = "PNG file (*.png)|*.png";
      saveFileDialog.ShowDialog();
      fileName = saveFileDialog.FileName;
      if (fileName != ""){
          using (var stream = File.Create(fileName)) encoder.Save(stream);
      }            
  }
```
# Lời kết
---
Qua project nhỏ này thì nhóm chúng em đã học được khá nhiều điều, về cách sử dụng Arduino, đọc thông số cảm biến, thiết kế mạch, in mạch, xử lí dữ liệu, ôn tập lại kiến thức về C#.
