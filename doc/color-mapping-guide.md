# Guía de Mapeo de Colores para .NET MAUI

## Introducción
Esta guía establece los principios y la lógica para mapear colores desde Colors.xaml a los componentes en Styles.xaml en aplicaciones .NET MAUI. El objetivo es mantener una consistencia visual y una experiencia de usuario coherente.

## Estructura de Colores Base

### Colores Primarios
```xaml
<Color x:Key="PrimaryCl">#512BD4</Color>
<Color x:Key="SecondaryCl">#2B0B98</Color>
<Color x:Key="AccentCl">#2D6FCC</Color>
```

### Colores Semánticos
```xaml
<Color x:Key="ErrorCl">#FF0000</Color>
<Color x:Key="SuccessCl">#00FF00</Color>
<Color x:Key="WarningCl">#FFFF00</Color>
```

### Colores Neutrales
```xaml
<Color x:Key="ForegroundCl">#F7F5FF</Color>
<Color x:Key="BackgroundCl">#23135E</Color>
<Color x:Key="Complementary1Cl">#E1E1E1</Color>
<Color x:Key="Complementary2Cl">#ACACAC</Color>
<Color x:Key="Complementary3Cl">#6E6E6E</Color>
```

## Principios de Mapeo

### 1. Elementos Principales de Interacción

#### Buttons
- **Color de Fondo**: PrimaryCl
- **Color de Texto**: ForegroundCl
- **Estado Deshabilitado**: Complementary2Cl
- **Estado Hover**: SecondaryCl

*Justificación*: Los botones son elementos primarios de interacción y deben destacar claramente en la interfaz.

#### Switches y Checkboxes
- **Estado Activo**: AccentCl
- **Estado Inactivo**: Complementary2Cl
- **Estado Deshabilitado**: Complementary3Cl
- **Borde**: Complementary1Cl

*Justificación*: Estos elementos necesitan estados claramente diferenciados pero sin competir visualmente con los botones principales.

### 2. Elementos de Entrada y Texto

#### Entry/Editor
- **Color de Texto**: ForegroundCl
- **Placeholder**: Complementary2Cl
- **Borde**: Complementary1Cl
- **Fondo**: BackgroundCl (con transparencia)

*Justificación*: Mantiene la legibilidad mientras proporciona suficiente contraste para la entrada de datos.

#### Labels
- **Texto Principal**: ForegroundCl
- **Texto Secundario**: Complementary1Cl
- **Texto Deshabilitado**: Complementary3Cl

*Justificación*: Establece una clara jerarquía visual en la información presentada.

### 3. Elementos de Navegación

#### Shell/NavigationPage
- **Barra de Navegación**: BackgroundCl
- **Texto Activo**: ForegroundCl
- **Texto Inactivo**: Complementary2Cl

*Justificación*: Proporciona un marco consistente para la navegación mientras mantiene la legibilidad.

#### TabbedPage
- **Tab Activo**: PrimaryCl
- **Tab Inactivo**: Complementary3Cl
- **Texto Tab**: ForegroundCl

*Justificación*: Facilita la identificación del contexto actual de navegación.

### 4. Elementos de Estado del Sistema

#### Indicadores de Progreso
- **Progreso Normal**: PrimaryCl
- **Progreso Éxito**: SuccessCl
- **Progreso Error**: ErrorCl
- **Fondo**: Complementary1Cl

*Justificación*: Comunica claramente el estado del sistema mientras mantiene la consistencia con el esquema de color general.

#### Mensajes de Estado
- **Error**: ErrorCl
- **Éxito**: SuccessCl
- **Advertencia**: WarningCl
- **Información**: AccentCl

*Justificación*: Utiliza colores semánticos estándar para una comunicación clara del estado.

## Consideraciones Adicionales

### Accesibilidad
- Mantener un ratio de contraste mínimo de 4.5:1 para texto pequeño
- Usar combinaciones de colores que sean distinguibles para personas con daltonismo
- Proporcionar indicadores adicionales además del color para estados importantes

### Modo Oscuro
- Invertir la relación figura-fondo usando las variantes Dark de los colores
- Mantener la misma lógica de mapeo pero ajustando los valores de luminosidad
- Considerar reducir la saturación de algunos colores para evitar fatiga visual

### Consistencia
- Mantener el mismo color para el mismo tipo de acción a través de toda la aplicación
- Usar variaciones de opacidad para crear jerarquía visual sin introducir nuevos colores
- Limitar el uso de colores accent para elementos que realmente necesitan destacar

## Mejores Prácticas
1. Evitar usar colores directamente en los elementos - siempre referir a los recursos de color
2. Mantener la cantidad de colores al mínimo necesario para una interfaz clara
3. Documentar cualquier desviación de esta guía de mapeo
4. Probar las combinaciones de colores en diferentes dispositivos y condiciones de luz
5. Considerar el impacto de los colores en diferentes tamaños de pantalla

---

## Notas de Implementación
- Usar StaticResource para referencias de color que no cambiarán durante la ejecución
- Usar DynamicResource para colores que pueden cambiar (ej: tema claro/oscuro)
- Considerar el uso de valores de opacidad para crear variaciones sutiles sin introducir nuevos colores
- Mantener un sistema de nomenclatura consistente al añadir nuevos colores