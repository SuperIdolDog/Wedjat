# Wedjat

https://www.bilibili.com/video/BV18V2rBmEg8/

基于 Halcon + 海康工业相机(MVS) + 西门子 PLC (TIA Portal V17) 开发的工业视觉检测上位机，适用于 PCB 流水线自动化缺陷检测演示与学习。

项目说明
本项目为简化开源版，仅实现基础视觉检测流程与界面交互。

检测算法：固定写死的差分对比算法，仅支持 PCB 缺孔检测。

不包含：HDevEngine 动态调用、Halcon 深度学习模块、复杂缺陷检测能力。

复杂缺陷（鼠咬、短路、开路、缺件等）必须依赖深度学习与高质量数据集。

项目为早期开发版本（赶进度版），未使用事件总线 / 发布订阅模式，界面耦合度较高，代码注释不完善。

环境依赖
海康相机 MVS 及对应 DLL、
MVTec Halcon（需自行放置许可证文件）、
西门子博图 TIA Portal V17、
串口调试助手（辅助调试）、
PCB 素材（PCB 样本等素材不随项目开源，请前往：北京大学智能机器人开放实验室 申请 或 自行在网络搜索获取）。

适用场景
工业视觉上位机开发学习、
PCB 检测系统流程演示、
机器视觉 + PLC 联动教学。


<img width="566" height="419" alt="image" src="https://github.com/user-attachments/assets/b880a96f-bf58-470d-854e-674978172a3d" />

<video src="https://private-user-images.githubusercontent.com/216304413/523443494-104e74ed-5adc-4790-abed-fb9f2f5a2976.mp4?jwt=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbSIsImtleSI6ImtleTUiLCJleHAiOjE3NjUwOTk0MjEsIm5iZiI6MTc2NTA5OTEyMSwicGF0aCI6Ii8yMTYzMDQ0MTMvNTIzNDQzNDk0LTEwNGU3NGVkLTVhZGMtNDc5MC1hYmVkLWZiOWYyZjVhMjk3Ni5tcDQ_WC1BbXotQWxnb3JpdGhtPUFXUzQtSE1BQy1TSEEyNTYmWC1BbXotQ3JlZGVudGlhbD1BS0lBVkNPRFlMU0E1M1BRSzRaQSUyRjIwMjUxMjA3JTJGdXMtZWFzdC0xJTJGczMlMkZhd3M0X3JlcXVlc3QmWC1BbXotRGF0ZT0yMDI1MTIwN1QwOTE4NDFaJlgtQW16LUV4cGlyZXM9MzAwJlgtQW16LVNpZ25hdHVyZT04MjQ0ZmU5NzRiODEwYTUyYjM3MmYyMGFiZGY0MGVlNGViMzE4MjQ4OTlmOWRhNGU4Y2IyNDNmZmE0OWY4ODgxJlgtQW16LVNpZ25lZEhlYWRlcnM9aG9zdCJ9.RMy_ie3qieqKmZv5LvIDPs11sPJFBvMhQH8UvBQP-Tg" data-canonical-src="https://private-user-images.githubusercontent.com/216304413/523443494-104e74ed-5adc-4790-abed-fb9f2f5a2976.mp4?jwt=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbSIsImtleSI6ImtleTUiLCJleHAiOjE3NjUwOTk0MjEsIm5iZiI6MTc2NTA5OTEyMSwicGF0aCI6Ii8yMTYzMDQ0MTMvNTIzNDQzNDk0LTEwNGU3NGVkLTVhZGMtNDc5MC1hYmVkLWZiOWYyZjVhMjk3Ni5tcDQ_WC1BbXotQWxnb3JpdGhtPUFXUzQtSE1BQy1TSEEyNTYmWC1BbXotQ3JlZGVudGlhbD1BS0lBVkNPRFlMU0E1M1BRSzRaQSUyRjIwMjUxMjA3JTJGdXMtZWFzdC0xJTJGczMlMkZhd3M0X3JlcXVlc3QmWC1BbXotRGF0ZT0yMDI1MTIwN1QwOTE4NDFaJlgtQW16LUV4cGlyZXM9MzAwJlgtQW16LVNpZ25hdHVyZT04MjQ0ZmU5NzRiODEwYTUyYjM3MmYyMGFiZGY0MGVlNGViMzE4MjQ4OTlmOWRhNGU4Y2IyNDNmZmE0OWY4ODgxJlgtQW16LVNpZ25lZEhlYWRlcnM9aG9zdCJ9.RMy_ie3qieqKmZv5LvIDPs11sPJFBvMhQH8UvBQP-Tg" controls="controls" muted="muted" class="d-block rounded-bottom-2 border-top width-fit" style="max-height:640px; min-height: 200px">

  </video>

<img width="1196" height="743" alt="image" src="https://github.com/user-attachments/assets/c8ff1269-de3e-471a-814f-3f8cca36cdda" />
<img width="2153" height="1297" alt="image" src="https://github.com/user-attachments/assets/fba7ee26-49ab-48d1-9df8-1a32311ccab5" />
<img width="2152" height="1294" alt="image" src="https://github.com/user-attachments/assets/06708894-5391-4c70-8991-f4c6c1c080a6" />

<img width="2159" height="1306" alt="image" src="https://github.com/user-attachments/assets/7b8398ff-88d5-495f-8c7b-5687521f9bc1" />

<img width="2154" height="1301" alt="image" src="https://github.com/user-attachments/assets/9c01c25e-f400-4403-a10d-d74196d6160a" />
<img width="2152" height="1298" alt="image" src="https://github.com/user-attachments/assets/ef718da8-a6d1-40fb-a9ac-e708d39f00b6" />



