# Wind Particle System Setup Guide

## Автоматическая настройка
Скрипт `WindZone2D.cs` автоматически настроит большинство параметров при запуске.

## Ручная настройка для идеального результата

### 1. Создание объекта WindZone2D
1. Создайте пустой GameObject в сцене
2. Добавьте компонент `WindZone2D`
3. Добавьте `BoxCollider2D` (автоматически добавится)
4. Настройте размер BoxCollider2D под нужную зону ветра

### 2. Создание Particle System
1. Создайте дочерний объект с Particle System
2. Назначьте его в поле `Wind Particles` компонента WindZone2D

### 3. Настройка Particle System для "нитей ветра"

#### Main Module:
- Duration: 5.00
- Looping: ✓
- Start Lifetime: 2-3
- Start Speed: 0 (скорость задаётся через Velocity over Lifetime)
- Start Size: 0.05-0.1
- Start Color: Белый с альфой 0.3-0.5
- Gravity Modifier: 0
- Simulation Space: World
- Max Particles: 100-200

#### Emission:
- Rate over Time: 30-50

#### Shape:
- Shape: Box
- Scale: Совпадает с размером BoxCollider2D
- Emit from: Volume

#### Velocity over Lifetime:
- ✓ Enabled
- Space: World
- Linear X: Направление ветра по X * силу (например, 5 для ветра вправо)
- Linear Y: Направление ветра по Y * силу

#### Size over Lifetime (опционально):
- ✓ Enabled
- Curve: Начинается с 1, плавно уменьшается к 0.5 к концу жизни

#### Color over Lifetime (опционально):
- ✓ Enabled
- Alpha: Fade in в начале, fade out в конце
- Gradient: От прозрачного → полупрозрачный → прозрачный

#### Renderer:
- Render Mode: **Stretched Billboard**
- Length Scale: **2.0-3.0** (длина нитей)
- Velocity Scale: **0.3-0.5** (растяжение по скорости)
- Material: WindMaterial

### 4. Настройка материала
1. Выберите `Assets/Particle Systems/WindMaterial.mat`
2. Назначьте шейдер `Particles/WindStreaks`
3. Настройте цвет (рекомендуется светло-голубой/белый с альфой 0.3-0.5)
4. Fade: 0.3-0.5

### 5. Создание текстуры (опционально)
Для лучшего эффекта создайте простую текстуру:
- Размер: 256x32 пикселей
- Белая горизонтальная линия с градиентом по краям
- Формат: PNG с альфа-каналом
- Назначьте в Main Tex материала WindMaterial

## Параметры WindZone2D

### Wind Settings:
- **Wind Direction**: Направление ветра (Vector2)
- **Wind Strength**: Сила ветра (5-15 рекомендуется)
- **Use Gust Effect**: Включить порывы ветра
- **Gust Frequency**: Частота порывов (1-3)
- **Gust Strength**: Дополнительная сила порывов (2-5)

## Советы для лучшего визуала

1. **Для горизонтального ветра**: 
   - Wind Direction: (1, 0) или (-1, 0)
   - Velocity Linear X: ±5 до ±10

2. **Для диагонального ветра**:
   - Wind Direction: (1, 0.5) нормализованный
   - Настройте Velocity X и Y соответственно

3. **Для сильного шторма**:
   - Увеличьте Rate over Time до 80-100
   - Length Scale: 3-4
   - Wind Strength: 15-20
   - Включите Gust Effect

4. **Для лёгкого бриза**:
   - Rate over Time: 20-30
   - Length Scale: 1.5-2
   - Wind Strength: 3-5
   - Alpha: 0.2-0.3

## Слои и производительность

- Установите Particle System на слой "Effects" или "Particles"
- Для множественных зон ветра используйте Object Pooling
- Max Particles не должен превышать 200 для мобильных устройств
