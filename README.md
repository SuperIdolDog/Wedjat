# Wedjat

https://www.bilibili.com/video/BV18V2rBmEg8/

流水线PCB缺陷检测上位机系统（
开源版本:
检测代码为固定代码（写死），
没有检测算法模块（没有HDevEngine(动态调用)）,
没有Halcon深度学习,
PCB相关素材请去北京大学智能机器人开放实验室网站申请或网上自行搜索获取
）

1、先下好软件（MVS、MVTec、串口调试助手、博图V17）
2、找MVS的dll，然后引用到项目中，找到本月MVTec的许可证，放到文件夹的license中，以及引用bin文件夹下的dll到项目中
3、万行屎山，写完4个月了，当时还是赶出来的也没有好好写注释。现在一看代码，我自己也懵逼（跨界面调用方法肯定要总线路由最好的，我刚开始写的时候就光想着就几个需要跨页面传值的方法（前期没考虑，后期难维护））。
4、开源版本的检测部分意义不大，没有融入深度学习，直接使用差分写死的(我只能实现缺孔的检测，其他缺陷不太适合使用差分。很麻烦，想要检测那些鼠咬短路开路啥的，肯定是要深度学习的。反正我是提取不出来这些缺陷，也可能是这个数据集拍的它就不好，硬件才是基础)）。


<img width="566" height="419" alt="image" src="https://github.com/user-attachments/assets/b880a96f-bf58-470d-854e-674978172a3d" />

<video src="https://private-user-images.githubusercontent.com/216304413/523443494-104e74ed-5adc-4790-abed-fb9f2f5a2976.mp4?jwt=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbSIsImtleSI6ImtleTUiLCJleHAiOjE3NjUwOTk0MjEsIm5iZiI6MTc2NTA5OTEyMSwicGF0aCI6Ii8yMTYzMDQ0MTMvNTIzNDQzNDk0LTEwNGU3NGVkLTVhZGMtNDc5MC1hYmVkLWZiOWYyZjVhMjk3Ni5tcDQ_WC1BbXotQWxnb3JpdGhtPUFXUzQtSE1BQy1TSEEyNTYmWC1BbXotQ3JlZGVudGlhbD1BS0lBVkNPRFlMU0E1M1BRSzRaQSUyRjIwMjUxMjA3JTJGdXMtZWFzdC0xJTJGczMlMkZhd3M0X3JlcXVlc3QmWC1BbXotRGF0ZT0yMDI1MTIwN1QwOTE4NDFaJlgtQW16LUV4cGlyZXM9MzAwJlgtQW16LVNpZ25hdHVyZT04MjQ0ZmU5NzRiODEwYTUyYjM3MmYyMGFiZGY0MGVlNGViMzE4MjQ4OTlmOWRhNGU4Y2IyNDNmZmE0OWY4ODgxJlgtQW16LVNpZ25lZEhlYWRlcnM9aG9zdCJ9.RMy_ie3qieqKmZv5LvIDPs11sPJFBvMhQH8UvBQP-Tg" data-canonical-src="https://private-user-images.githubusercontent.com/216304413/523443494-104e74ed-5adc-4790-abed-fb9f2f5a2976.mp4?jwt=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbSIsImtleSI6ImtleTUiLCJleHAiOjE3NjUwOTk0MjEsIm5iZiI6MTc2NTA5OTEyMSwicGF0aCI6Ii8yMTYzMDQ0MTMvNTIzNDQzNDk0LTEwNGU3NGVkLTVhZGMtNDc5MC1hYmVkLWZiOWYyZjVhMjk3Ni5tcDQ_WC1BbXotQWxnb3JpdGhtPUFXUzQtSE1BQy1TSEEyNTYmWC1BbXotQ3JlZGVudGlhbD1BS0lBVkNPRFlMU0E1M1BRSzRaQSUyRjIwMjUxMjA3JTJGdXMtZWFzdC0xJTJGczMlMkZhd3M0X3JlcXVlc3QmWC1BbXotRGF0ZT0yMDI1MTIwN1QwOTE4NDFaJlgtQW16LUV4cGlyZXM9MzAwJlgtQW16LVNpZ25hdHVyZT04MjQ0ZmU5NzRiODEwYTUyYjM3MmYyMGFiZGY0MGVlNGViMzE4MjQ4OTlmOWRhNGU4Y2IyNDNmZmE0OWY4ODgxJlgtQW16LVNpZ25lZEhlYWRlcnM9aG9zdCJ9.RMy_ie3qieqKmZv5LvIDPs11sPJFBvMhQH8UvBQP-Tg" controls="controls" muted="muted" class="d-block rounded-bottom-2 border-top width-fit" style="max-height:640px; min-height: 200px">

  </video>

<img width="1196" height="743" alt="image" src="https://github.com/user-attachments/assets/c8ff1269-de3e-471a-814f-3f8cca36cdda" />
<img width="2153" height="1297" alt="image" src="https://github.com/user-attachments/assets/fba7ee26-49ab-48d1-9df8-1a32311ccab5" />
<img width="2152" height="1294" alt="image" src="https://github.com/user-attachments/assets/06708894-5391-4c70-8991-f4c6c1c080a6" />

<img width="2159" height="1306" alt="image" src="https://github.com/user-attachments/assets/7b8398ff-88d5-495f-8c7b-5687521f9bc1" />

<img width="2154" height="1301" alt="image" src="https://github.com/user-attachments/assets/9c01c25e-f400-4403-a10d-d74196d6160a" />
<img width="2152" height="1298" alt="image" src="https://github.com/user-attachments/assets/ef718da8-a6d1-40fb-a9ac-e708d39f00b6" />



