# Guía de Desarrollo

## Filosofía de Código Sostenible

Siguiendo los principios de **Carlos Blé**, el código debe ser:

### 1. Auto-explicativo
```csharp
// ❌ MAL
public void Proc(int n) { ... }

// ✅ BIEN
public void IniciarSesionParaMesa(MesaId mesaId) { ... }
```

### 2. Funciones pequeñas
- Máximo 20 líneas
- Una sola responsabilidad
- Un nivel de abstracción

```csharp
// ✅ BIEN
public void IniciarSesion()
{
    ValidarMesaDisponible();
    CrearNuevaSesion();
    GuardarCambios();
}
```

### 3. Sin comentarios innecesarios
El código debe hablar por sí mismo. Los comentarios solo cuando sean absolutamente necesarios para explicar el "por qué", no el "qué".

```csharp
// ❌ MAL
// Incrementa el contador en 1
contador++;

// ✅ BIEN (cuando es necesario explicar lógica de negocio compleja)
// El IVA se aplica después del descuento según normativa fiscal española
var precioConImpuestos = AplicarIVA(precioConDescuento);
```

## TDD - Test Driven Development

### Ciclo Red-Green-Refactor

1. **RED**: Escribe un test que falle
2. **GREEN**: Escribe el código mínimo para que pase
3. **REFACTOR**: Mejora el código sin romper tests

### Ejemplo completo:

#### 1. RED - Test que falla
```csharp
[Fact]
public void IniciarSesion_MesaOcupada_DeberiaLanzarExcepcion()
{
    // Arrange
    var mesa = new Mesa(new MesaId(5));
    mesa.IniciarSesion();

    // Act
    var act = () => mesa.IniciarSesion();

    // Assert
    act.Should().Throw<DomainException>();
}
```

#### 2. GREEN - Código mínimo
```csharp
public void IniciarSesion()
{
    if (EstaOcupada)
        throw new DomainException("Mesa ocupada");

    SesionActiva = SesionMesa.Crear();
}
```

#### 3. REFACTOR - Mejorar
```csharp
public void IniciarSesion()
{
    ValidarMesaDisponible();
    SesionActiva = SesionMesa.Crear();
}

private void ValidarMesaDisponible()
{
    if (EstaOcupada)
    {
        throw new DomainException(
            $"La mesa {Id.Value} ya tiene una sesión activa.");
    }
}
```

## Arquitectura Hexagonal

### Flujo de dependencias

```
[API] → [Application] → [Domain]
         ↓
   [Infrastructure]
```

### Reglas:
1. **Domain** no depende de NADIE
2. **Application** solo depende de Domain
3. **Infrastructure** implementa puertos de Application
4. **API** coordina todo

### Ejemplo práctico:

```csharp
// ✅ Domain - Sin dependencias externas
public class Mesa
{
    public void IniciarSesion() { ... }
}

// ✅ Application - Define puerto (interfaz)
public interface IMesaRepository
{
    Task<Mesa?> ObtenerPorId(MesaId id);
}

// ✅ Infrastructure - Implementa puerto
public class PostgresMesaRepository : IMesaRepository
{
    private readonly DbContext _context;

    public async Task<Mesa?> ObtenerPorId(MesaId id)
    {
        return await _context.Mesas
            .FirstOrDefaultAsync(m => m.Id == id);
    }
}

// ✅ API - Coordina
public class MesasController
{
    private readonly IniciarSesionMesaUseCase _useCase;

    [HttpPost("{id}/iniciar-sesion")]
    public async Task<IActionResult> Iniciar(int id)
    {
        var result = await _useCase.Execute(id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
```

## Convenciones de Nombrado

### C# Backend

```csharp
// Clases y Métodos: PascalCase
public class MesaRepository { }
public void IniciarSesion() { }

// Variables locales y parámetros: camelCase
var numeroMesa = 5;
public void Procesar(int numeroMesa) { }

// Constantes: PascalCase
public const int MaximoMesas = 50;

// Interfaces: IPrefijo
public interface IMesaRepository { }

// Propiedades privadas: _camelCase
private readonly IMesaRepository _repository;
```

### TypeScript Frontend

```typescript
// Tipos e Interfaces: PascalCase
interface MesaData { }
type ResultadoOperacion = { }

// Variables y funciones: camelCase
const numeroMesa = 5
function iniciarSesion() { }

// Constantes: UPPER_SNAKE_CASE
const API_BASE_URL = 'http://...'

// Componentes React: PascalCase
function MenuPage() { }
```

## Value Objects

Los Value Objects encapsulan validaciones y comportamiento:

```csharp
// ✅ BIEN
public sealed record MesaId
{
    public int Value { get; }

    public MesaId(int value)
    {
        if (value <= 0)
            throw new DomainException("El número debe ser positivo");

        Value = value;
    }
}

// Uso
var mesaId = new MesaId(5); // ✅ Válido
var mesaId = new MesaId(-1); // ❌ Lanza excepción
```

### Ventajas:
- Validación centralizada
- Imposible tener estados inválidos
- Semántica clara
- Type safety

## Casos de Uso

Un caso de uso = Una clase = Una responsabilidad

```csharp
public class IniciarSesionMesaUseCase
{
    private readonly IMesaRepository _repository;

    public IniciarSesionMesaUseCase(IMesaRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<SesionDto>> Execute(int numeroMesa)
    {
        try
        {
            var mesa = await ObtenerMesa(numeroMesa);
            IniciarSesion(mesa);
            await GuardarCambios(mesa);
            return CrearRespuestaExitosa(mesa);
        }
        catch (DomainException ex)
        {
            return Result<SesionDto>.Failure(ex.Message);
        }
    }

    // Métodos privados pequeños y específicos
    private async Task<Mesa> ObtenerMesa(int numero) { ... }
    private void IniciarSesion(Mesa mesa) { ... }
    private async Task GuardarCambios(Mesa mesa) { ... }
    private Result<SesionDto> CrearRespuestaExitosa(Mesa mesa) { ... }
}
```

## Testing - Convenciones

### Naming
```csharp
[Fact]
public void MetodoAProbar_Escenario_ResultadoEsperado()
{
    // Arrange
    var sut = CrearSistemaAProbar();

    // Act
    var resultado = sut.Ejecutar();

    // Assert
    resultado.Should().Be(esperado);
}
```

### Categorías de tests

```csharp
// Tests Unitarios - Rápidos, aislados
[Fact]
public void MesaId_NumeroNegativo_LanzaExcepcion() { }

// Tests de Integración - Base de datos real
[Fact]
public async Task Repository_GuardarMesa_PersisteDatos() { }

// Tests E2E - Todo el flujo
[Fact]
public async Task Cliente_EscanearQR_VeMenu() { }
```

## Estructura de commits

### Formato
```
tipo(scope): descripción breve

Descripción detallada del cambio y por qué fue necesario.

BREAKING CHANGE: si aplica
```

### Ejemplos
```bash
feat(domain): add Mesa and SesionMesa entities with TDD

Implement domain entities following DDD principles:
- Mesa entity with session management
- SesionMesa value object
- Unit tests with 100% coverage

test(application): add IniciarSesionMesaUseCase tests

Implement use case tests following outside-in TDD approach.
All tests pass with InMemory repository implementation.
```

## Checklist antes de commit

- [ ] Todos los tests pasan
- [ ] Cobertura > 80%
- [ ] Sin warnings de compilación
- [ ] Linter sin errores
- [ ] Nombres auto-explicativos
- [ ] Funciones pequeñas (< 20 líneas)
- [ ] Sin código comentado
- [ ] Sin magic numbers
- [ ] Inyección de dependencias correcta

## Seguridad

### Validación de entrada
```csharp
// ✅ Siempre validar en Value Objects
public record Email
{
    public string Value { get; }

    public Email(string value)
    {
        if (!IsValidEmail(value))
            throw new DomainException("Email inválido");

        Value = value;
    }
}
```

### Nunca exponer información sensible
```csharp
// ❌ MAL
catch (Exception ex)
{
    return BadRequest(ex.ToString());
}

// ✅ BIEN
catch (DomainException ex)
{
    _logger.LogWarning(ex, "Error de dominio");
    return BadRequest(new { error = ex.Message });
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error inesperado");
    return StatusCode(500, "Error interno del servidor");
}
```

## Recursos Adicionales

- [Clean Code - Robert C. Martin](https://www.amazon.com/Clean-Code-Handbook-Software-Craftsmanship/dp/0132350882)
- [Código Sostenible - Carlos Blé](https://savvily.es/libros/codigo-sostenible/)
- [Domain-Driven Design - Eric Evans](https://www.domainlanguage.com/ddd/)
- [Hexagonal Architecture - Alistair Cockburn](https://alistair.cockburn.us/hexagonal-architecture/)
