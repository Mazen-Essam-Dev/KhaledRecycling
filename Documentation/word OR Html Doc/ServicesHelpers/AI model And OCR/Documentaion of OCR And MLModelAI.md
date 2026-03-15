# 🚗 MLModel AI Controller Module Documentation

## videos 
https://www.youtube.com/watch?v=fxyIefV8cuw

```
When making a photo model 
Image Classification
When it gets break down in training, look below at the error screen and read where it is standing because no error appears, but he writes what it is doing below
Most likely, there will be a problem downloading the Manol download file, putting it in this path, and creating what is required to do the correct training
Close and open the project again and tell him to do training again. it will know that it does not need to download the file again 
https://aka.ms/mlnet-resources/meta/resnet_v2_50_299.meta
Download it and move it to this path 
C:\Users\hp\AppData\Local\Temp\MLNET\resnet_v2_50_299.meta
It is better to leave the project outside the C
```

```
عند عمل موديل صور 
Image Classification
لما يعلق في التدريب بص تحت في شاشة الايرور واقرأ هو واقف فين لأنه لا يظهر خطأ لاكنه يكتب ما يفعله تحت
في الاغلب ستحدث مشكلة في تحميل ملف حمله مانول وحطه في المسار ده وانشء المطلوب علشان يعمل تدريب صح
واقفل وافتح المشروع من جديد وقوله يعمل تدريب من جديد هيعرف انه مش محتاج ينزل الملف مرة اخرى 
https://aka.ms/mlnet-resources/meta/resnet_v2_50_299.meta
حمله وانقله للمسار ده 
C:\Users\hp\AppData\Local\Temp\MLNET\resnet_v2_50_299.meta
ويفضل تخلي المشروع بره ال c احسن
```
## ORC Project
- we extracted text data from (img and pdf) as english and arabic text using lipraries free in .Net

### First Moduel
extracted text data from (img) as english and arabic text using lipraries SixLabors.ImageSharp -- Tesseract

#### SixLabors.ImageSharp
used for sharpen and discrepancy and convert img to gray and then Crop data section

#### Tesseract
used for get data from images as arabic and english **first download language files as ara.traineddata -- eng.traineddata **

*link 1 for little files more faster but low quality https://github.com/tesseract-ocr/tessdata *
*link 2 for big files more slower but high quality https://github.com/tesseract-ocr/tessdata_best * 
>>>> **put downloaded files ara,eng and put them in folder with name `tessdata` and put this folder in project which has this package installed**
