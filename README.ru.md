# DotBimExport

[![JetBrains Rider](https://img.shields.io/badge/JetBrains-Rider-blue.svg)](https://www.jetbrains.com/rider)
[![License MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE.md)
[![Revit 2016-2027](https://img.shields.io/badge/Revit-2016--2027-blue.svg)](https://www.autodesk.com/products/revit/overview)

[English](README.md)

Плагин Revit для экспорта 3D-видов в формат [dotbim](https://github.com/paireks/dotbim) (`.bim`).
Экспорт реализован библиотекой `dosymep.Revit.DotBim` в этом репозитории.

## Возможности

- Команда на ленте с окном настроек экспорта
- Выбор 3D-вида
- Опциональный экспорт связей, параметров элементов и сведений о проекте
- Фильтр по категориям модели
- Выбор языка имён (`LanguageType` Revit)
- Локализация интерфейса: английский (по умолчанию) и русский — по языку Revit
- Поддержка Revit 2016–2027 (включая `2026.5`)

## Установка

1. Скачайте `DotBimExport.{version}.zip` из [Releases](https://github.com/dosymep/revit-dotbim/releases) (версия должна
   совпадать с годом Revit).
2. Распакуйте архив в папку Addins для этой версии, например:

   `%AppData%\Autodesk\Revit\Addins\2025\`

3. Перезапустите Revit. На ленте появится панель **dotbim** с командой **Export DotBim** / **Экспорт DotBim**.

Структура архива:

```text
DotBimExport.2025.zip
  DotBimExport.addin
  DotBimExport/
    DotBimExport.dll
    dosymep.Revit.DotBim.dll
    ru/
      DotBimExport.resources.dll
    …
```

## Использование

1. Откройте документ с хотя бы одним **3D-видом** (не шаблоном).
2. На панели **dotbim** нажмите **Export DotBim** / **Экспорт DotBim**.
3. В окне настроек укажите:
    - 3D-вид;
    - язык имён;
    - нужно ли выгружать информацию элементов, документа и связи;
    - категории для экспорта.
4. Подтвердите экспорт и выберите путь к файлу `.bim`.

Язык интерфейса окна и ленты берётся из языка Revit: для русского — `ru`, для остальных — английский.

### API библиотеки

`dosymep.Revit.DotBim` предоставляет метод расширения для `Document`, если экспорт нужно вызвать из своего кода:

```csharp
View view = uiApplication.ActiveUIDocument.ActiveView;
Document document = uiApplication.ActiveUIDocument.Document;

document.Export("path/to/sample.bim",
    new DotBimOptions {
        View = view,
        WithLinks = true,
        WithElementInfo = true,
        WithDocumentInfo = true,
        CategoryIds = null,
        NameLanguage = LanguageType.English_USA
    });
```

| Параметр           | Тип                               | По умолчанию  | Описание                                       |
|--------------------|-----------------------------------|---------------|------------------------------------------------|
| `View`             | `View`                            | обязательный  | Вид для экспорта (должен быть `View3D`)        |
| `WithLinks`        | `bool`                            | `false`       | Включать связанные модели                      |
| `WithElementInfo`  | `bool`                            | `false`       | Включать параметры элементов                   |
| `WithDocumentInfo` | `bool`                            | `false`       | Включать сведения о проекте                    |
| `CategoryIds`      | `IReadOnlyCollection<ElementId>?` | `null`        | Категории для экспорта; `null` — все категории |
| `NameLanguage`     | `LanguageType`                    | `English_USA` | Язык имён при выгрузке                         |

## Сборка

В репозитории есть приложение [ModularPipelines](https://github.com/thomhurst/ModularPipelines) в `build/`. Оно очищает
`bin`/`obj`, восстанавливает решение, собирает `DotBimExport` для выбранных версий Revit в `bin/<version>/DotBimExport/`
и упаковывает add-in zip в `artifacts/`.

Этапы пайплайна:

1. **Clean** — удаление корневых `bin/` и `artifacts/`, а также `bin`/`obj` проектов в `src/`
2. **Restore** — восстановление NuGet-пакетов решения
3. **Build** — сборка плагина (и библиотеки) для каждой версии Revit
4. **Package** — упаковка каждой версии в layout add-in Revit в `artifacts/`

### Требования

- [.NET SDK](https://dotnet.microsoft.com/download) по версии из `global.json` (net10)
- Пакеты ссылок Revit API через `Autodesk.Revit.Sdk.Refs`

### Поддерживаемые версии Revit

По умолчанию (локальный пайплайн и GitHub Actions): **2016–2027**, включая **2026.5**.

### Запуск пайплайна сборки

Из корня репозитория:

```bash
dotnet run --project build
```

### Переменные окружения

| Переменная        | По умолчанию                  | Описание                                                                          |
|:------------------|:------------------------------|:----------------------------------------------------------------------------------|
| `CONFIGURATION`   | `Release`                     | Конфигурация MSBuild                                                              |
| `REVIT_VERSIONS`  | `2016,…,2027` (вкл. `2026.5`) | Одна или несколько версий Revit. Разделители: запятая, точка с запятой или пробел |
| `MAX_PARALLELISM` | `max(1, ProcessorCount / 4)`  | Максимум параллельных задач упаковки                                              |

#### PowerShell

```powershell
$env:CONFIGURATION = "Release"
$env:REVIT_VERSIONS = "2024,2025"
$env:MAX_PARALLELISM = "2"
dotnet run --project build
```

#### Bash

```bash
CONFIGURATION=Release REVIT_VERSIONS=2024,2025 MAX_PARALLELISM=2 dotnet run --project build
```

### Быстрая сборка одной версии

Без пайплайна:

```bash
dotnet build revit-dotbim.slnx -c D2024
```

Доступные конфигурации: `D2016` … `D2027` (и `D2026.5`).

### Результаты сборки

```text
bin/
  2024/
    DotBimExport/
  2025/
    DotBimExport/
artifacts/
  DotBimExport.2024.zip
  DotBimExport.2025.zip
```

### Версионирование

Базовая версия задаётся в `Directory.Build.props` (`2.0.0`).

| Контекст                    | Пример версии сборки            |
|:----------------------------|:--------------------------------|
| Локальная / CI-сборка ветки | `2.0.0-{commit}.{revitVersion}` |
| GitHub-тег `v2.1.0`         | `2.1.0.{revitVersion}`          |

На тегах `v*` GitHub Actions также создаёт релиз и загружает `artifacts/*`.

## Пример

См. [`sample/sample.bim`](sample/sample.bim).

## Участие в разработке

См. [CONTRIBUTING.md](CONTRIBUTING.md).

## Благодарности

См. [CREDITS.md](CREDITS.md).

## Лицензия

Проект распространяется по [лицензии MIT](LICENSE.md).
