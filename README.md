# Wedjat

https://www.bilibili.com/video/BV18V2rBmEg8/

基于WinForm(AntdUI：https://gitee.com/AntdUI/AntdUI) + Halcon + 海康威视工业相机 MV-CU120-10GC(MVS) + 西门子 PLC 1214C DC/DC/DC(TIA Portal V17) 开发的工业视觉检测上位机，适用于 PCB 流水线自动化缺陷检测演示与学习。

项目说明



本项目为简化开源版，仅实现基础视觉检测流程与界面交互，开源版无深度学习和调参模块。

检测算法：固定写死的差分对比算法，仅支持 PCB 缺孔检测（为什么没有用差分检测其他缺陷？我菜，我提取不出来其他缺陷，都是连一块的）。

不包含：HDevEngine 动态调用、Halcon 深度学习模块、复杂缺陷检测能力。

复杂缺陷（鼠咬、短路、开路等）必须依赖深度学习与高质量数据集。

项目为早期开发版本（赶进度版），未使用事件总线 / 发布订阅模式，界面耦合度较高，代码注释不完善。

增删改查没有做，是因为各种数据本身就应该从Mes中拉取，做在上位机系统上不合适，一个这么简单的上位机还需要权限管理吗？本身就是个可视化控制工具。

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

业务流程



通信准备在配置好串口 / 网口等通信参数后，系统先与 PLC、相机、扫码枪等硬件建立连接。

工单获取使用扫码枪扫描工单二维码，系统自动解析并加载对应工单信息。

模板导入根据工单的PCB，导入对应的 PCB 检测模板，完成检测参数配置（其实我想做的是哪种自动切换检测模板的，实际上每块PCB都应该有二维码用来追溯信息的，但是又没实物，被迫无奈放弃）。

流水线控制支持链式传送带分段速度调节；检测工位传送带由红外传感器自动控制（相机的硬触发，也就是根据红外线传感器的得电失电状态（0和1）来判断是否进行拍照的） —— 遮挡时启动，无遮挡时停止。

自动视觉检测工件到位后延时触发相机拍照，系统通过差分算法完成 PCB 缺孔缺陷检测。

结果判定与反馈



检测通过：播放 Pass 提示音，记录合格信息

检测失败：播放 Fail 提示音，记录缺陷信息

最终将检测结果与数据上传至 MES 系统


<img width="566" height="419" alt="image" src="https://github.com/user-attachments/assets/b880a96f-bf58-470d-854e-674978172a3d" />

<video src="https://private-user-images.githubusercontent.com/216304413/523443494-104e74ed-5adc-4790-abed-fb9f2f5a2976.mp4?jwt=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbSIsImtleSI6ImtleTUiLCJleHAiOjE3NjUwOTk0MjEsIm5iZiI6MTc2NTA5OTEyMSwicGF0aCI6Ii8yMTYzMDQ0MTMvNTIzNDQzNDk0LTEwNGU3NGVkLTVhZGMtNDc5MC1hYmVkLWZiOWYyZjVhMjk3Ni5tcDQ_WC1BbXotQWxnb3JpdGhtPUFXUzQtSE1BQy1TSEEyNTYmWC1BbXotQ3JlZGVudGlhbD1BS0lBVkNPRFlMU0E1M1BRSzRaQSUyRjIwMjUxMjA3JTJGdXMtZWFzdC0xJTJGczMlMkZhd3M0X3JlcXVlc3QmWC1BbXotRGF0ZT0yMDI1MTIwN1QwOTE4NDFaJlgtQW16LUV4cGlyZXM9MzAwJlgtQW16LVNpZ25hdHVyZT04MjQ0ZmU5NzRiODEwYTUyYjM3MmYyMGFiZGY0MGVlNGViMzE4MjQ4OTlmOWRhNGU4Y2IyNDNmZmE0OWY4ODgxJlgtQW16LVNpZ25lZEhlYWRlcnM9aG9zdCJ9.RMy_ie3qieqKmZv5LvIDPs11sPJFBvMhQH8UvBQP-Tg" data-canonical-src="https://private-user-images.githubusercontent.com/216304413/523443494-104e74ed-5adc-4790-abed-fb9f2f5a2976.mp4?jwt=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbSIsImtleSI6ImtleTUiLCJleHAiOjE3NjUwOTk0MjEsIm5iZiI6MTc2NTA5OTEyMSwicGF0aCI6Ii8yMTYzMDQ0MTMvNTIzNDQzNDk0LTEwNGU3NGVkLTVhZGMtNDc5MC1hYmVkLWZiOWYyZjVhMjk3Ni5tcDQ_WC1BbXotQWxnb3JpdGhtPUFXUzQtSE1BQy1TSEEyNTYmWC1BbXotQ3JlZGVudGlhbD1BS0lBVkNPRFlMU0E1M1BRSzRaQSUyRjIwMjUxMjA3JTJGdXMtZWFzdC0xJTJGczMlMkZhd3M0X3JlcXVlc3QmWC1BbXotRGF0ZT0yMDI1MTIwN1QwOTE4NDFaJlgtQW16LUV4cGlyZXM9MzAwJlgtQW16LVNpZ25hdHVyZT04MjQ0ZmU5NzRiODEwYTUyYjM3MmYyMGFiZGY0MGVlNGViMzE4MjQ4OTlmOWRhNGU4Y2IyNDNmZmE0OWY4ODgxJlgtQW16LVNpZ25lZEhlYWRlcnM9aG9zdCJ9.RMy_ie3qieqKmZv5LvIDPs11sPJFBvMhQH8UvBQP-Tg" controls="controls" muted="muted" class="d-block rounded-bottom-2 border-top width-fit" style="max-height:640px; min-height: 200px">

  </video>

<img width="1196" height="743" alt="image" src="https://github.com/user-attachments/assets/c8ff1269-de3e-471a-814f-3f8cca36cdda" />
<img width="2153" height="1297" alt="image" src="https://github.com/user-attachments/assets/fba7ee26-49ab-48d1-9df8-1a32311ccab5" />
<img width="2152" height="1294" alt="image" src="https://github.com/user-attachments/assets/06708894-5391-4c70-8991-f4c6c1c080a6" />

<img width="2159" height="1306" alt="image" src="https://github.com/user-attachments/assets/7b8398ff-88d5-495f-8c7b-5687521f9bc1" />

<img width="2154" height="1301" alt="image" src="https://github.com/user-attachments/assets/9c01c25e-f400-4403-a10d-d74196d6160a" />
<img width="2152" height="1298" alt="image" src="https://github.com/user-attachments/assets/ef718da8-a6d1-40fb-a9ac-e708d39f00b6" />



