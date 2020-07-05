# ATBSolutions
Программные решения для компании Airport Technical Brushes.

## DxfToNcConverter
Создание nc-программ на основе существующих dxf-файлов.

Программа представлена в виде однофайловой консольной утилиты для ОС Windows 7 и выше.

### Системные требования
- Операционная система: **Windows 7 и выше**.
- Установленный [.NET Core Runtime](https://dotnet.microsoft.com/download/dotnet-core/current/runtime)

### Принцип работы
Утилита просматривает рабочую директорию на предмет наличия `*.dxf` файлов.  
Все найденные `*.dxf` файлы читаются и проверяются. В случае, если `*.dxf` файл не соответствует требованиям - он удаляется из процесса создания nc-программы для этого файла (см. раздел **[Требования к чертежам](#Требования-к-чертежам)**).  

`*.dxf` файл - это файл чертежа AutoCad 2018+.

Самый большой круг (Circle) на чертеже определяет внешний диаметр щетки.  
Отверстия для сверления следует указывать с использованием замкнутых полилиний (Polyline) внутри самого большого круга (Circle).  
Каждая точка полилинии (Polyline Vertex) является отверстием для сверления.


### Аргументы
#### Рабочая директория
Чтобы программа работала в произвольной директории следует передать нужную директорию в качестве первого аргумента при выполнении утилиты.  
Например: `C:\dxfs\ATB.DxfToNcConverter.exe "C:\Program Files\dxfs_folder"`  
По-умолчанию рабочая директория соответствует директории запуска утилиты.

#### Время опускания сверла
Для того, чтобы во всех создаваемых NC-программах регулировать время опускания сверла, требуется передать в аргументы флаг `-hdt <hdt>` или `-hole-drill-time <hdt>` с указанием времени в секундах для опускания сверла.  
Например: `C:\dxfs\ATB.DxfToNcConverter.exe -hdt 2`  
По-умолчанию время опускания сверла равно 1.5 секундам.

#### Позиция после окончания программы
Чтобы настроить позицию после окончания программы, требуется передать в аргументы утилиты флаг `-epx <epx>` или `--end-position-x <epx>` с указанием позиции по оси X, на которую должен стать подвижный механизм.  
Например: `C:\dxfs\ATB.DxfToNcConverter.exe -epx 200`  
По-умолчанию позиция после окончания программы равна 300.

#### Начальный отступ
Чтобы настроить начальный отступ, требуется передать в аргументы утилиты флаг `-spxo <spxo>` или `--start-point-x-offset <spxo>` с указанием начального отсупа по оси X.
Например: `C:\dxfs\ATB.DxfToNcConverter.exe -spxo -60`
По-умолчанию начальный отступ равен -50.

#### Режимы работы
##### Режим отладки
Режим отладки запускается передачей аргумента `-d` или `--debug` при запуске утилиты.  
Он позволяет получить детальную информацию о выполнении утилиты в консольном окне.  
Данный режим также включает логирование процесса в файл в папку `logs` в исполняемой директории.  
Также, данный режим оставляет открытым окно программы после завершения.  

##### Режим Что-Если
Режим Что-Если запускается передачей аргумента `-wi` или `--what-if` при запуске утилиты.  
Он позволяет получить детальную информацию о выполнении утилиты в консольном окне.  
**В данном режиме не будет производиться реального создания nc-программ.**  
Также, данный режим оставляет открытым окно программы после завершения.  

### Использование
1. [Скачать](https://github.com/picolino/ATBSolutions/releases) последнюю версию утилиты ATB.DxfToNcConverter
1. Положить скачанный .exe файл в папку с имеющимися файлами чертежей (`*.dxf` файлами)
1. Запустить ATB.DxfToNcConverter.
1. Дождаться завершения работы утилиты.
    1. В случае, если в процессе работы произошла ошибка, программа напишет сообщение об ошибке и останется открытой.
1. После завершения работы программы в рабочей директории будут находиться `*.nc` файлы с именами от соответствующих `*.dxf` файлов.

### Требования к чертежам
- Чертеж должен содержать хотя-бы один круг (Circle)
- Все полилинии (Polyline) должны быть замкнутыми
- Все точки всех полилиний (Polyline) должны находиться внутри самого большого круга (Circle)

### FAQ
**Q: После выполнения утилиты нет созданного `*.nc` файла**  
A: Скорее всего обработка соответствующего `*.dxf` файла была остановлена из-за несоответствия требованиям.

**Q: Утилита ищет `*.dxf` файлы в подпапках?**  
A: Нет, утилита работает только в рабочей директории.

### Контакты
Для решения проблем с использованием утилиты следует обращаться к автору через сайт [picolino.dev](https://picolino.dev).
