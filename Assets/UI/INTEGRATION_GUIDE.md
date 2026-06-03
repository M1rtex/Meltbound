# UI Toolkit Integration Guide

## Созданные файлы:

### UXML (структура UI):
- `Assets/UI/GameUI.uxml` - основной игровой интерфейс (кнопки Pause/Restart + слайдер здоровья)
- `Assets/UI/PauseMenu.uxml` - меню паузы
- `Assets/UI/StartMenu.uxml` - стартовое меню
- `Assets/UI/RestartMenu.uxml` - меню рестарта (Game Over)

### USS (стили):
- `Assets/UI/GameUI.uss` - все стили для UI

### C# Scripts:
- `Assets/Scripts/UIManager.cs` - полностью переписан для UI Toolkit
- `Assets/Scripts/UIController.cs` - упрощен, управляет только health bar
- `Assets/Scripts/HealthBar.cs` - убрана зависимость от UGUI

---

## Пошаговая настройка в Unity:

### Шаг 1: Создайте PanelSettings (если еще нет)
1. В Project: правый клик → Create → UI Toolkit → Panel Settings
2. Назовите его `DefaultPanelSettings`
3. Настройте:
   - **Scale Mode**: Scale With Screen Size
   - **Reference Resolution**: 1920 x 1080
   - **Screen Match Mode**: Match Width Or Height
   - **Match**: 0.5

### Шаг 2: Создайте GameObject для основного UI
1. В Hierarchy: правый клик → Create Empty
2. Назовите `GameUI_UIToolkit`
3. Добавьте компонент **UIDocument**:
   - **Source Asset**: перетащите `GameUI.uxml`
   - **Panel Settings**: выберите `DefaultPanelSettings`
4. Добавьте компонент **UIController** (скрипт)

### Шаг 3: Создайте GameObject для меню паузы
1. В Hierarchy: правый клик → Create Empty
2. Назовите `PauseMenu_UIToolkit`
3. Добавьте компонент **UIDocument**:
   - **Source Asset**: перетащите `PauseMenu.uxml`
   - **Panel Settings**: выберите `DefaultPanelSettings`
   - **Sort Order**: 10 (чтобы было поверх основного UI)

### Шаг 4: Создайте GameObject для стартового меню
1. В Hierarchy: правый клик → Create Empty
2. Назовите `StartMenu_UIToolkit`
3. Добавьте компонент **UIDocument**:
   - **Source Asset**: перетащите `StartMenu.uxml`
   - **Panel Settings**: выберите `DefaultPanelSettings`
   - **Sort Order**: 10

### Шаг 5: Создайте GameObject для меню рестарта
1. В Hierarchy: правый клик → Create Empty
2. Назовите `RestartMenu_UIToolkit`
3. Добавьте компонент **UIDocument**:
   - **Source Asset**: перетащите `RestartMenu.uxml`
   - **Panel Settings**: выберите `DefaultPanelSettings`
   - **Sort Order**: 10

### Шаг 6: Настройте UIManager
1. Найдите GameObject с компонентом **UIManager** (или создайте новый)
2. В инспекторе UIManager заполните поля:
   - **Game UI Document**: перетащите `GameUI_UIToolkit`
   - **Pause Menu Document**: перетащите `PauseMenu_UIToolkit`
   - **Start Menu Document**: перетащите `StartMenu_UIToolkit`
   - **Restart Menu Document**: перетащите `RestartMenu_UIToolkit`

### Шаг 7: Настройте HealthBar
1. Найдите GameObject с компонентом **HealthBar**
2. В инспекторе HealthBar:
   - **UI Controller**: перетащите `GameUI_UIToolkit`
   - **UI Manager**: перетащите GameObject с UIManager
   - Удалите старое поле **Slider** (больше не нужно)
   - Удалите старое поле **Restart UI** (больше не нужно)

### Шаг 8: Удалите старый UGUI Canvas
1. Найдите старый Canvas в Hierarchy
2. Удалите его (или отключите для тестирования)

### Шаг 9: Проверьте EventSystem
1. Убедитесь, что в сцене есть **EventSystem**
2. Он должен иметь компонент **InputSystemUIInputModule** (не StandaloneInputModule)
3. Если его нет: правый клик в Hierarchy → UI → Event System

---

## Структура UI в сцене (итоговая):

```
Scene
├── GameUI_UIToolkit (UIDocument + UIController)
├── PauseMenu_UIToolkit (UIDocument)
├── StartMenu_UIToolkit (UIDocument)
├── RestartMenu_UIToolkit (UIDocument)
├── UIManager (UIManager script)
├── HealthBar (HealthBar script)
└── EventSystem (InputSystemUIInputModule)
```

---

## Что изменилось:

### UIManager:
- Полностью переписан для работы с UI Toolkit
- Управляет 4 UIDocument вместо GameObject
- Использует `DisplayStyle.Flex/None` вместо `SetActive()`
- Все кнопки подписываются через `clicked +=`

### HealthBar:
- Убрана зависимость от UGUI Slider
- Работает только через UIController
- Вызывает `uiManager.ShowRestartMenu()` при Game Over

### UIController:
- Упрощен - управляет только ProgressBar
- Больше не управляет кнопками (это делает UIManager)

---

## Проверка работы:

1. **Запустите игру** - должно появиться стартовое меню "MELTBOUND"
2. **Нажмите Start Game** - меню исчезнет, начнется игра
3. **Проверьте health bar** - должен убывать в левом верхнем углу
4. **Нажмите Pause** - должно появиться меню паузы
5. **Нажмите Resume** - игра продолжится
6. **Дождитесь 0 HP** - должно появиться меню "GAME OVER"

---

## Troubleshooting:

### ProgressBar отображается как точка:
- Убедитесь, что используется обновленный `GameUI.uss`
- Проверьте, что USS файл подключен к UXML (должен подключиться автоматически)

### Кнопки не работают:
- Проверьте, что все UIDocument правильно назначены в UIManager
- Убедитесь, что в сцене есть EventSystem с InputSystemUIInputModule

### Меню не появляются:
- Проверьте Sort Order у UIDocument (меню должны быть выше основного UI)
- Убедитесь, что Panel Settings назначен всем UIDocument

### Ошибки в консоли:
- Проверьте, что все имена элементов в UXML совпадают с кодом
- Убедитесь, что USS файл находится в той же папке, что и UXML
