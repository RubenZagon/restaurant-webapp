# Plan de Desarrollo - Sistema de Comandas Digital para Restaurante

## 📋 Resumen Ejecutivo

### Objetivo
Desarrollar una aplicación web progresiva (PWA) que permita a los clientes de un restaurante realizar pedidos y pagos desde su mesa mediante código QR, siguiendo principios de Clean Architecture y Código Sostenible.

### Stack Tecnológico
- **Backend**: C# (.NET 8) con arquitectura hexagonal
- **Frontend**: React 18 con TypeScript
- **Base de datos**: PostgreSQL (principal) + Redis (caché)
- **Infraestructura**: Docker + Kubernetes (Digital Ocean)
- **CI/CD**: GitHub Actions
- **Metodología**: Outside-in TDD, DDD, SOLID

---

## 🏗️ FASE 1: Fundación y Arquitectura Base (2-3 semanas)

### 1.1 Configuración del Entorno de Desarrollo

#### Backend (C#/.NET)
```
/src
  /RestaurantApp.Domain           # Entidades y lógica de negocio
    /Entities
    /ValueObjects
    /DomainServices
    /Exceptions
  
  /RestaurantApp.Application       # Casos de uso
    /UseCases
    /DTOs
    /Ports (Interfaces)
    
  /RestaurantApp.Infrastructure    # Implementaciones externas
    /Persistence
    /Payment
    /Notifications
    
  /RestaurantApp.API              # Controladores y configuración
    /Controllers
    /Middleware
    /Configuration
```

#### Frontend (React)
```
/src
  /domain                         # Modelos de dominio
  /application                    # Casos de uso y servicios
  /infrastructure                 # Adaptadores externos
    /api
    /storage
  /presentation                   # Componentes UI
    /components
    /pages
    /hooks
```

### 1.2 Configuración Inicial

**Tareas:**
- [ ] Crear repositorios en GitHub con estructura monorepo
- [ ] Configurar Docker Compose para desarrollo local
- [ ] Implementar pre-commit hooks (formato, linting, tests)
- [ ] Configurar entornos: development, staging, production
- [ ] Documentar convenciones de código siguiendo a Carlos Blé

**Tests Primera Iteración (Outside-in):**
1. Test E2E: "Un cliente puede ver la pantalla de bienvenida al escanear QR"
2. Test Integración: "El sistema identifica correctamente la mesa desde el QR"
3. Test Unitario: "Mesa Value Object valida formato correcto"

### 1.3 CI/CD Pipeline Básico

```yaml
# .github/workflows/main.yml
- Build & Test en cada PR
- SonarQube para análisis de código
- Deploy automático a staging
- Deploy manual a producción
```

---

## 🎯 FASE 2: Core del Dominio - Gestión de Mesas y Menú (2 semanas)

### 2.1 Bounded Context: Mesa Management

**Agregados:**
- `Mesa`: Número, estado (libre/ocupada), sesión activa
- `SesiónMesa`: ID único, timestamp inicio, estado

**Casos de Uso:**
- `IniciarSesiónMesa`: Cliente escanea QR y se crea sesión
- `ObtenerEstadoMesa`: Verificar disponibilidad
- `FinalizarSesiónMesa`: Al completar pago

**Tests (Outside-in):**
```csharp
// Test E2E
"Dado que soy un cliente
 Cuando escaneo el QR de la mesa 5
 Entonces veo el menú personalizado para mesa 5"

// Test Caso de Uso
"IniciarSesiónMesa debe crear sesión única por mesa"

// Test Dominio
"Mesa no puede tener dos sesiones activas simultáneas"
```

### 2.2 Bounded Context: Catálogo de Productos

**Entidades:**
- `Producto`: ID, nombre, descripción, precio, categoría
- `Categoría`: Entrantes, principales, bebidas, postres
- `Disponibilidad`: Stock, horario disponible

**Value Objects:**
- `Precio`: Validación de moneda y formato
- `Alérgenos`: Lista de alérgenos con iconos

**Implementación Frontend:**
- Componentes atómicos (Button, Card, Price)
- Composición de componentes (ProductCard, CategoryList)
- Custom hooks para gestión de estado

---

## 🛒 FASE 3: Sistema de Pedidos (3 semanas)

### 3.1 Bounded Context: Gestión de Pedidos

**Agregado Pedido:**
```csharp
public class Pedido : AggregateRoot
{
    public PedidoId Id { get; private set; }
    public MesaId Mesa { get; private set; }
    public List<LineaPedido> Lineas { get; private set; }
    public EstadoPedido Estado { get; private set; }
    public Money Total { get; private set; }
    
    // Comportamientos siguiendo DDD
    public void AgregarProducto(Producto producto, int cantidad)
    public void EliminarLinea(LineaPedidoId id)
    public void ConfirmarPedido()
}
```

**Casos de Uso:**
- `CrearPedido`
- `AgregarProductoAPedido`
- `ModificarCantidadProducto`
- `EliminarProductoDePedido`
- `CalcularTotalPedido`
- `EnviarPedidoACocina`

### 3.2 Implementación Frontend - Carrito de Compra

**Componentes React (Código Sostenible):**
```typescript
// Componente contenedor (Smart)
const CartContainer: FC = () => {
  // Lógica de negocio mediante custom hooks
  const { items, addItem, removeItem, total } = useCart();
  
  return <CartPresentation items={items} onAdd={addItem} />;
};

// Componente presentacional (Dumb)
const CartPresentation: FC<CartProps> = ({ items, onAdd }) => {
  // Solo renderizado, sin lógica
};
```

### 3.3 Tests de Integración

```typescript
describe('Flujo completo de pedido', () => {
  it('debe permitir agregar productos y calcular total', async () => {
    // Given: Mesa 5 tiene sesión activa
    // When: Agrego 2 cervezas y 1 tapa
    // Then: Total = suma de precios * cantidades
  });
});
```

---

## 💳 FASE 4: Sistema de Pagos (2 semanas)

### 4.1 Integración con Pasarela de Pago (Mock)

**Puerto (Hexagonal Architecture):**
```csharp
public interface IPaymentGateway
{
    Task<PaymentResult> ProcessPayment(PaymentRequest request);
    Task<PaymentStatus> CheckStatus(PaymentId id);
}

// Implementación Mock para demo
public class MockPaymentGateway : IPaymentGateway
{
    // Simula respuestas de Stripe/PayPal
}
```

### 4.2 Flujo de Pago

1. Cliente solicita pagar
2. Sistema calcula total con impuestos
3. Genera intento de pago
4. Mock gateway aprueba (demo)
5. Actualiza estado pedido
6. Notifica a cocina

**Consideraciones de Seguridad:**
- Tokenización de tarjetas (simulado)
- PCI compliance (documentado)
- Logs de auditoría

---

## 👨‍🍳 FASE 5: Panel de Cocina y Camareros (2 semanas)

### 5.1 Dashboard en Tiempo Real

**WebSockets con SignalR:**
```csharp
public class KitchenHub : Hub
{
    public async Task NotifyNewOrder(Order order)
    {
        await Clients.Group("kitchen").SendAsync("NewOrder", order);
    }
}
```

**Interfaz Cocina:**
- Vista kanban de pedidos (Pendiente → Preparando → Listo)
- Tiempo estimado por pedido
- Alertas de modificaciones
- Estadísticas del servicio

### 5.2 Sistema de Notificaciones

- Push notifications cuando pedido está listo
- SMS/Email de confirmación (opcional)
- WebSocket para actualizaciones en tiempo real

---

## 🚀 FASE 6: DevOps y Despliegue (1 semana)

### 6.1 Dockerización

```dockerfile
# Backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "RestaurantApp.API.dll"]
```

### 6.2 Kubernetes en Digital Ocean

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: restaurant-backend
spec:
  replicas: 3
  selector:
    matchLabels:
      app: restaurant-backend
  template:
    spec:
      containers:
      - name: api
        image: registry.digitalocean.com/restaurant/backend:latest
        ports:
        - containerPort: 80
```

### 6.3 GitHub Actions CI/CD

```yaml
name: Deploy to Production
on:
  push:
    branches: [main]
jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - name: Run tests
        run: |
          dotnet test --collect:"XPlat Code Coverage"
          npm test -- --coverage
      
  deploy:
    needs: test
    runs-on: ubuntu-latest
    steps:
      - name: Deploy to Digital Ocean
        run: |
          doctl kubernetes cluster kubeconfig save restaurant
          kubectl apply -f k8s/
```

---

## 📊 FASE 7: Optimización y Monitoreo (1 semana)

### 7.1 Observabilidad

- **Logs**: Serilog + ELK Stack
- **Métricas**: Prometheus + Grafana
- **Tracing**: OpenTelemetry
- **Alertas**: PagerDuty integration

### 7.2 Optimizaciones de Rendimiento

- Implementar caché con Redis
- CDN para assets estáticos
- Lazy loading en frontend
- Paginación en listados
- Índices de base de datos

### 7.3 Tests de Carga

```bash
# K6 para pruebas de carga
k6 run --vus 100 --duration 30s load-test.js
```

---

## 🔒 FASE 8: Seguridad y Compliance (1 semana)

### 8.1 Seguridad

- Rate limiting por IP
- CORS configuración
- Sanitización de inputs
- Autenticación JWT para staff
- Encriptación de datos sensibles

### 8.2 GDPR Compliance

- Política de privacidad
- Consentimiento de cookies
- Derecho al olvido
- Exportación de datos

---

## 📈 FASE 9: Analytics y Mejoras (Continuo)

### 9.1 Analytics de Negocio

- Productos más vendidos
- Tiempos de preparación
- Horas pico
- Satisfacción del cliente

### 9.2 A/B Testing

- Diferentes layouts de menú
- Proceso de checkout
- Recomendaciones personalizadas

---

## 🎓 Mejores Prácticas a Mantener

### Código Sostenible (Carlos Blé)
- **Nombres reveladores**: Las variables y funciones expresan su intención
- **Funciones pequeñas**: Máximo 20 líneas, hacen una sola cosa
- **Sin comentarios**: El código es autoexplicativo
- **Tests como documentación**: Los tests explican el comportamiento

### Arquitectura Hexagonal
- **Dominio puro**: Sin dependencias del framework
- **Puertos y adaptadores**: Inversión de dependencias
- **Casos de uso explícitos**: Un caso de uso = una clase
- **Testing aislado**: Mocks solo en los bordes

### Outside-in TDD
1. Escribir test E2E que falla
2. Escribir test de integración que falla
3. Escribir tests unitarios que fallan
4. Implementar código mínimo para pasar tests
5. Refactorizar manteniendo tests en verde

---

## 📅 Cronograma Estimado

| Fase | Duración | Entregable Principal |
|------|----------|---------------------|
| 1. Fundación | 2-3 semanas | Arquitectura base + CI/CD |
| 2. Core Dominio | 2 semanas | Gestión mesas y menú |
| 3. Pedidos | 3 semanas | Sistema completo de pedidos |
| 4. Pagos | 2 semanas | Integración pasarela (mock) |
| 5. Panel Cocina | 2 semanas | Dashboard tiempo real |
| 6. DevOps | 1 semana | Despliegue producción |
| 7. Optimización | 1 semana | Monitoreo y métricas |
| 8. Seguridad | 1 semana | Hardening y compliance |
| **TOTAL** | **14-15 semanas** | **MVP Completo** |

---

## 🚦 Criterios de Éxito

- ✅ Cobertura de tests > 80%
- ✅ Deuda técnica < 5% (SonarQube)
- ✅ Tiempo de respuesta < 200ms (p95)
- ✅ Disponibilidad > 99.9%
- ✅ Zero downtime deployments
- ✅ Código mantenible (métrica de complejidad ciclomática < 10)

---

## 📝 Notas para Claude Sonnet 4.5

Al implementar cada fase con Sonnet 4.5, asegúrate de:

1. **Comenzar siempre con un test que falle** (Red-Green-Refactor)
2. **Mantener el dominio libre de frameworks** (Clean Architecture)
3. **Usar Value Objects para conceptos de negocio** (DDD)
4. **Aplicar el principio de responsabilidad única** (SOLID)
5. **Documentar decisiones arquitectónicas** (ADRs)
6. **Realizar code reviews automatizados** (Danger JS)

Este plan está diseñado para ser iterativo y adaptable. Cada fase produce valor de negocio y puede ser desplegada independientemente.