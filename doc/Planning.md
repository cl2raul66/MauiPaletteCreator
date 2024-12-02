# Planificación del Proyecto: MauiPaletteCreator

## Versión
1.0

## Objetivo
Crear una aplicación que permita a los desarrolladores generar y visualizar paletas de colores personalizadas para proyectos .NET MAUI, facilitando la integración de estilos y temas de color.

## Flujo de Uso

1. **Bienvenida**:
   - Pantalla de inicio con una bienvenida amigable.

2. **Seleccionar el Proyecto para Aplicar la Paleta de Colores (Opcional)**:
   - Ventaja: Si se selecciona un proyecto, se pueden definir automáticamente las versiones de .NET, las plataformas de visualización y simplificar el proceso de exportación y aplicación.
   - Flujo: Al seleccionar un proyecto, los pasos 2, 3 y 5 se configuran automáticamente. Esto optimiza el flujo de trabajo y minimiza la necesidad de configuraciones adicionales.

3. **Generar la Paleta de Colores**:
   - Seleccionar colores primarios y usar la API de Colormind.

4. **Visualización de la Galería en el Dispositivo según la Plataforma Seleccionada (Opcional)**:
   - Generar y ejecutar el proyecto GalleryPreview en la plataforma seleccionada.

5. **Exportar Ficheros o Aplicar Directamente**:
   - Opciones de exportar a un directorio o aplicar directamente al proyecto seleccionado.
   - Si no se seleccionó un proyecto al inicio, el usuario puede seleccionar un directorio para exportar los archivos.

## Estructura de Archivos Generados

- **Colores Compartidos (Comunes para Todas las Plataformas)**
  - `Resources\Styles\Colors.xaml`
  - `Resources\Styles\Styles.xaml`

- **iOS**
  - `Platforms\iOS\AppDelegate.cs`
  - `Resources\Styles\Colors.xaml`

- **Android**
  - `Platforms\Android\MainActivity.cs`
  - `Resources\Styles\Colors.xaml`

- **Windows**
  - `Platforms\Windows\App.xaml.cs`
  - `Resources\Styles\Colors.xaml`

## Ventajas

1. **Modularidad y Reutilización**
   - Uso de `ResourceDictionary.xaml` para organizar y reutilizar recursos de manera eficiente.

2. **Facilidad de Implementación y Visualización**
   - Galería de componentes dentro de la app para previsualización en tiempo real.
   - Proyecto temporal de .NET MAUI para visualizar la paleta en diferentes plataformas.

3. **Productividad y Eficiencia**
   - Exportación directa de ficheros de colores y estilos.
   - Integración manual en proyectos por parte de los desarrolladores para mayor flexibilidad.

## Requisitos

- SDK de .NET instalado en las plataformas Windows y macOS.
- Herramientas de desarrollo (e.g., Android Emulator, iOS Simulator) configuradas correctamente.

## Desafíos y Posibles Soluciones

### Generar la Paleta de Colores
- **Desafío**: Crear una interfaz de usuario intuitiva para seleccionar y personalizar colores.
- **Solución**: Utilizar la API de Colormind para generar paletas armoniosas y facilitar el uso.

### Elegir Versión de .NET del Proyecto (Opcional)
- **Desafío**: Asegurar compatibilidad con varias versiones de .NET MAUI.
- **Solución**: Implementar opciones de selección y pruebas de compatibilidad usando `dotnet --list-sdks`.

### Seleccionar las Plataformas de Visualización de la Galería (Opcional)
- **Desafío**: Asegurar que la galería se visualice correctamente en todas las plataformas soportadas.
- **Solución**: Generar un proyecto GalleryPreview y detectar plataformas soportadas mediante bibliotecas del SDK.

### Visualización de la Galería en el Dispositivo (Opcional)
- **Desafío**: Lograr una previsualización precisa de la galería en diferentes dispositivos sin tener que implementar múltiples entornos nativos.
- **Solución**: Configurar y ejecutar el proyecto GalleryPreview según la plataforma seleccionada, utilizando comandos `dotnet build` y `dotnet run`.

### Exportar Ficheros o Aplicar Directamente a un Proyecto .NET MAUI
- **Desafío**: Integrar de manera fluida los ficheros generados en proyectos existentes de .NET MAUI.
- **Solución**: Ofrecer una opción clara para exportar los ficheros o aplicar directamente al proyecto, utilizando bibliotecas del SDK para detectar rutas y realizar reemplazos necesarios.

## Estructura del Proyecto

```plaintext
MauiPaletteCreator/
├── MauiPaletteCreator.sln
├── src/
│   ├── MauiPaletteCreator/
│   │   ├── MauiPaletteCreator.csproj
│   │   ├── Program.cs
│   │   ├── Resources/
│   │   │   ├── Styles/
│   │   │   │   ├── Colors.xaml
│   │   │   │   ├── Styles.xaml
│   │   ├── MainPage.xaml
│   │   ├── MainPage.xaml.cs
│   │   ├── ViewModels/
│   │   │   ├── MainViewModel.cs
│   │   ├── Services/
│   │   │   ├── ColorService.cs
│   │   │   ├── SDKService.cs
│   │   │   ├── PlatformService.cs
│   │   ├── Models/
│   │   │   ├── ColorModel.cs
│   │   │   ├── SDKModel.cs
│   │   │   ├── PlatformModel.cs
│   ├── GalleryPreview/
│   │   ├── GalleryPreview.csproj
│   │   ├── MainPage.xaml
│   │   ├── MainPage.xaml.cs
│   │   ├── Resources/
│   │   │   ├── Styles/
│   │   │   │   ├── Colors.xaml
│   │   │   │   ├── Styles.xaml
│   │   ├── Platforms/
│   │   │   ├── iOS/
│   │   │   ├── Android/
│   │   │   ├── Windows/
│   │   │   ├── macOS/
├── Documentation/
│   ├── InstallationGuide.txt
│   ├── README.md
```

## Conclusión
Esta aplicación proporcionará una herramienta poderosa y fácil de usar para los desarrolladores de .NET MAUI, permitiéndoles crear y aplicar paletas de colores personalizadas de manera eficiente y efectiva.
