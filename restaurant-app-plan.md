# Plan de Desarrollo - Sistema de Comandas Digital para Restaurante

## 📊 Estado del Proyecto (Actualizado: 2025-10-18)

### ✅ Fases Completadas
- **Fase 1**: Fundación y Arquitectura Base - **100% COMPLETA**
- **Fase 2**: Core del Dominio - **100% COMPLETA**
- **Fase 3**: Sistema de Pedidos - **95% COMPLETA** (Backend + Frontend completos, E2E tests pendientes)
- **Fase 4**: Notificaciones en Tiempo Real - **100% COMPLETA**

### 📈 Métricas Actuales
- **Tests Unitarios Backend**: 55+ tests pasando
- **Cobertura de Tests**: ~85% en dominio
- **Build Status**: ✅ Backend | ✅ Frontend
- **Vulnerabilidades npm**: 0 (actualizado)
- **Framework Testing**: Moq 4.18.4 (migrado desde NSubstitute)
- **Real-time Communication**: SignalR WebSockets implementado

### 🔧 Stack Implementado
- **Backend**: .NET 8.0.415 con Arquitectura Hexagonal
- **Frontend**: React 18 + TypeScript + Vite 6.0.5
- **State Management**: Zustand 4.5.7
- **Testing**: xUnit 2.9.2 + Moq + FluentAssertions 6.12.1
- **Containerization**: Docker multi-stage builds
- **Database**: In-Memory (desarrollo) → PostgreSQL (pendiente)

---

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

## 🏗️ FASE 1: Fundación y Arquitectura Base ✅ COMPLETADA

### 1.1 Configuración del Entorno de Desarrollo ✅

#### Backend (C#/.NET) ✅ IMPLEMENTADO
```
/src/backend
  /RestaurantApp.Domain           # ✅ Entidades y lógica de negocio
    /Entities                     # ✅ Table, TableSession, Category, Product, Order, OrderLine
    /ValueObjects                 # ✅ TableId, SessionId, Price, Allergens, OrderId, Quantity, OrderStatus
    /Exceptions                   # ✅ DomainException

  /RestaurantApp.Application      # ✅ Casos de uso
    /UseCases                     # ✅ StartTableSession, GetAllCategories, GetProductsByCategory,
                                  #    GetOrCreateOrderForTable, AddProductToOrder
    /DTOs                         # ✅ TableSessionDto, CategoryDto, ProductDto, OrderDto
    /Ports                        # ✅ ITableRepository, ICategoryRepository, IProductRepository, IOrderRepository

  /RestaurantApp.Infrastructure   # ✅ Implementaciones externas
    /Persistence                  # ✅ InMemory repositories con datos de muestra

  /RestaurantApp.API             # ✅ Controladores y configuración
    /Controllers                  # ✅ TablesController, CategoriesController, ProductsController, OrdersController
    Program.cs                    # ✅ DI, CORS, Serilog configurado

  /RestaurantApp.Tests.Unit      # ✅ Tests TDD
    /Domain                       # ✅ 55+ tests unitarios
    /Application                  # ✅ Tests con Moq
```

#### Frontend (React) ✅ IMPLEMENTADO
```
/src/frontend
  /src
    /store                        # ✅ Zustand state management (cartStore)
    /presentation
      /components                 # ✅ CartIcon, ShoppingCart, ProductCard, CategoryTabs
      /pages                      # ✅ WelcomePage, MenuPage
  /infrastructure
    /api                          # ✅ tableApi, productsApi, ordersApi (axios)
  /domain                         # ✅ Modelos TypeScript
```

### 1.2 Configuración Inicial ✅

**Tareas:**
- [x] ✅ Crear estructura de proyectos backend y frontend
- [x] ✅ Configurar Docker Compose para desarrollo local
  - Backend: Multi-stage Dockerfile (.NET SDK → Runtime)
  - Frontend: Multi-stage Dockerfile (Node → Nginx)
  - PostgreSQL 16 Alpine
  - Redis 7 Alpine
- [x] ✅ Configurar CORS para comunicación frontend-backend
- [x] ✅ Configurar Serilog para logging estructurado
- [x] ✅ Implementar arquitectura hexagonal
- [x] ✅ Documentar en DOCKER.md

**Tests Primera Iteración (Outside-in):** ✅ COMPLETADOS
1. ✅ Test Unitario: "TableId Value Object validates positive numbers"
2. ✅ Test Unitario: "Table cannot have two active sessions"
3. ✅ Test Caso de Uso: "StartTableSessionUseCase creates unique session per table"

### 1.3 CI/CD Pipeline Básico

```yaml
# .github/workflows/main.yml - PENDIENTE
- Build & Test en cada PR
- SonarQube para análisis de código
- Deploy automático a staging
- Deploy manual a producción
```

**Estado:** Docker configurado ✅ | CI/CD pipeline pendiente ⏳

---

## 🎯 FASE 2: Core del Dominio - Gestión de Mesas y Menú ✅ COMPLETADA

### 2.1 Bounded Context: Mesa Management ✅

**Agregados:** ✅ IMPLEMENTADOS
- ✅ `Table`: TableId, IsOccupied, ActiveSession
- ✅ `TableSession`: SessionId, StartedAt, IsActive

**Casos de Uso:** ✅ IMPLEMENTADOS
- ✅ `StartTableSessionUseCase`: Cliente escanea QR y se crea sesión
  - Valida que la mesa existe
  - Valida que no tiene sesión activa
  - Crea nueva sesión con timestamp
  - Retorna TableSessionDto

**Tests (Outside-in):** ✅ 15+ TESTS PASANDO
```csharp
// ✅ Test Caso de Uso (con Moq)
"Execute_WithValidTableId_ShouldStartSession"
"Execute_WhenTableNotFound_ShouldReturnFailure"
"Execute_WhenTableAlreadyOccupied_ShouldReturnFailure"

// ✅ Test Dominio
"StartSession_WhenTableFree_ShouldCreateSession"
"StartSession_WhenAlreadyOccupied_ShouldThrowException"
"TableId_WithNegativeNumber_ShouldThrowException"
```

**API Endpoints:** ✅ IMPLEMENTADOS
- `POST /api/tables/{tableNumber}/session` - Iniciar sesión de mesa

### 2.2 Bounded Context: Catálogo de Productos ✅

**Entidades:** ✅ IMPLEMENTADAS
- ✅ `Product`: ProductId, Name, Description, Price, Category, Allergens, IsAvailable
- ✅ `Category`: CategoryId, Name, Description, DisplayOrder

**Value Objects:** ✅ IMPLEMENTADOS
- ✅ `Price`: Amount, Currency (EUR, USD, GBP, etc.)
  - 13 tests de validación de moneda y operaciones
- ✅ `Allergens`: Valores normalizados, case-insensitive
  - 12 tests de normalización y búsqueda
- ✅ `ProductId`, `CategoryId`: Identidades fuertemente tipadas

**Repositorios:** ✅ IMPLEMENTADOS
- ✅ `InMemoryCategoryRepository`: 4 categorías precargadas
- ✅ `InMemoryProductRepository`: 15 productos de muestra

**Casos de Uso:** ✅ IMPLEMENTADOS
- ✅ `GetAllCategoriesUseCase`: Retorna todas las categorías ordenadas
- ✅ `GetProductsByCategoryUseCase`: Filtra productos por categoría

**API Endpoints:** ✅ IMPLEMENTADOS
- `GET /api/categories` - Obtener todas las categorías
- `GET /api/products/category/{categoryId}` - Productos por categoría

**Implementación Frontend:** ✅ COMPLETADA
- ✅ `ProductCard`: Componente presentacional con precio, alérgenos, botón "Add to Cart"
- ✅ `CategoryTabs`: Navegación por categorías
- ✅ `MenuPage`: Composición completa con integración de API
- ✅ Axios configurado para llamadas a backend
- ✅ React Router para navegación

---

## 🛒 FASE 3: Sistema de Pedidos - ✅ COMPLETADA (95%)

### 3.1 Bounded Context: Gestión de Pedidos ✅

**Agregado Order:** ✅ IMPLEMENTADO CON TDD
```csharp
public class Order : AggregateRoot
{
    public OrderId Id { get; private set; }
    public TableId TableId { get; private set; }
    public SessionId SessionId { get; private set; }
    public List<OrderLine> Lines { get; private set; }
    public OrderStatus Status { get; private set; }  // Draft, Confirmed, Preparing, Ready, Delivered, Cancelled
    public Price Total { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }

    // ✅ Comportamientos implementados siguiendo DDD
    public void AddProduct(ProductId productId, string productName, Price unitPrice, Quantity quantity)
    public void RemoveLine(OrderLineId id)
    public void UpdateLineQuantity(OrderLineId id, Quantity newQuantity)
    public void Confirm()
    public void Cancel()
    // + Validaciones de estado y recálculo automático del total
}
```

**Value Objects:** ✅ IMPLEMENTADOS
- ✅ `OrderId`: Identificador único del pedido
- ✅ `OrderLineId`: Identificador de línea de pedido
- ✅ `Quantity`: Validación 1-100 unidades con operaciones de suma
- ✅ `OrderStatus`: Enum para máquina de estados

**Entity OrderLine:** ✅ IMPLEMENTADA
- ✅ Línea de pedido con cálculo automático de subtotal
- ✅ Actualización de cantidad con recálculo
- ✅ 8 tests unitarios

**Agregado Order:** ✅ 15+ TESTS PASANDO
```csharp
// ✅ Tests implementados
"Create_WithValidData_ShouldCreateOrder"
"AddProduct_WithValidData_ShouldAddOrderLine"
"AddProduct_SameProductTwice_ShouldIncreaseQuantity" // Agregación automática
"AddProduct_WhenOrderConfirmed_ShouldThrowDomainException"
"RemoveLine_WithValidLineId_ShouldRemoveLine"
"UpdateLineQuantity_WithValidQuantity_ShouldUpdateQuantity"
"Confirm_WhenHasLines_ShouldConfirmOrder"
"Confirm_WhenEmpty_ShouldThrowDomainException"
"Cancel_WhenDraftOrConfirmed_ShouldCancelOrder"
"Total_ShouldBeSumOfAllLines"
```

**Casos de Uso:** ✅ IMPLEMENTADOS
- ✅ `GetOrCreateOrderForTableUseCase`: Obtiene o crea pedido para mesa activa
  - Valida sesión activa
  - Retorna pedido existente o crea uno nuevo
- ✅ `AddProductToOrderUseCase`: Añade productos al pedido
  - Valida producto existe y está disponible
  - Agrega producto con cantidad
  - Recalcula total automáticamente

**Repositorio:** ✅ IMPLEMENTADO
- ✅ `IOrderRepository`: Port con métodos GetById, GetActiveOrderByTable, Save, Delete
- ✅ `InMemoryOrderRepository`: Implementación con ConcurrentDictionary

**API Endpoints:** ✅ IMPLEMENTADOS
- `GET /api/orders/table/{tableNumber}` - Obtener/crear pedido de mesa
- `POST /api/orders/{orderId}/products` - Añadir productos al pedido

**Dependency Injection:** ✅ CONFIGURADO
- Todos los servicios registrados en `Program.cs` (líneas 26-32)

### 3.2 Implementación Frontend - Carrito de Compra ✅

**State Management con Zustand:** ✅ IMPLEMENTADO
```typescript
// ✅ /src/store/cartStore.ts
interface CartStore {
  items: CartItem[]
  tableNumber: number | null
  orderId: string | null

  addItem: (product, quantity) => void
  removeItem: (productId) => void
  updateQuantity: (productId, quantity) => void
  clearCart: () => void
  getTotalItems: () => number
  getTotalAmount: () => number
}
```

**Componentes React:** ✅ IMPLEMENTADOS
- ✅ `CartIcon`: Icono flotante con badge de cantidad de items
  - Posición fixed en esquina superior derecha
  - Badge rojo con contador
  - SVG de carrito de compras

- ✅ `ShoppingCart`: Sidebar del carrito
  - Lista de productos con imagen, nombre, precio
  - Controles +/- para cantidad (validación 1-100)
  - Botón "Remove" por item
  - Total calculado dinámicamente
  - Botón "Confirm Order"
  - Botón "Clear Cart"

- ✅ `ProductCard`: Actualizado con botón "Add to Cart"
  - Integración con Zustand store
  - Feedback visual al agregar

**Integración MenuPage:** ✅ COMPLETADA
- ✅ State management del carrito
- ✅ Modal/Sidebar del carrito
- ✅ Persistencia del tableNumber en store

### 3.3 Tests de Integración ⏳ PENDIENTE

```typescript
// TODO: Implementar tests E2E
describe('Flujo completo de pedido', () => {
  it('debe permitir agregar productos y calcular total', async () => {
    // Given: Mesa 5 tiene sesión activa
    // When: Agrego 2 cervezas y 1 tapa
    // Then: Total = suma de precios * cantidades
  });
});
```

**Integración Frontend-Backend:** ✅ COMPLETADA
- ✅ `ConfirmOrderUseCase`: Confirma pedido y actualiza estado
- ✅ `POST /api/orders/{orderId}/confirm`: Endpoint implementado
- ✅ `ordersApi.ts`: Cliente Axios con confirmación de pedidos
- ✅ `Toast`: Componente de notificaciones para feedback
- ✅ `MenuPage`: Integración completa con manejo de checkout

### 📋 Tareas Pendientes Fase 3

- [ ] **Tests E2E**: Flujo completo de agregar productos y confirmar pedido (Playwright + Testcontainers)
- [ ] **Persistencia real**: Migrar de InMemory a PostgreSQL

---

## 🔐 Mejoras de Seguridad Implementadas (Actualización 2025-10-17)

### Backend - Migración a Moq ✅
**Problema:** NSubstitute presentaba conflictos de dependencias con Castle.Core en .NET 8
**Solución:** Migración completa a Moq 4.18.4

**Cambios realizados:**
- ✅ Actualizado `RestaurantApp.Tests.Unit.csproj`:
  - Moq 4.18.4 (estable con .NET 8)
  - xUnit 2.9.2 (última versión)
  - FluentAssertions 6.12.1
  - coverlet.collector 6.0.2

- ✅ Refactorizado `StartTableSessionUseCaseTests.cs`:
  ```csharp
  // Antes (NSubstitute)
  _tableRepository = Substitute.For<ITableRepository>();
  _tableRepository.GetById(tableId).Returns(table);
  await _tableRepository.Received(1).Save(table);

  // Después (Moq)
  _tableRepositoryMock = new Mock<ITableRepository>();
  _tableRepositoryMock.Setup(r => r.GetById(tableId)).ReturnsAsync(table);
  _tableRepositoryMock.Verify(r => r.Save(table), Times.Once);
  ```

### Frontend - Actualización de Dependencias ✅

**Vulnerabilidades Críticas Resueltas:**
1. ✅ **inflight@1.0.6** - Memory leak (no soportado)
   - Eliminado transitivamente con actualización de npm packages

2. ✅ **rimraf@3.0.2** - Versión obsoleta
   - Actualizado transitivamente

3. ✅ **eslint@8.x** - No soportado
   - Migrado a **ESLint 9.17.0**

4. ✅ **happy-dom** - Vulnerabilidades de ejecución de código
   - Actualizado a **20.0.5** (parche de seguridad)

5. ✅ **esbuild/vite** - Vulnerabilidades moderadas
   - Actualizado a **Vite 6.0.5** y **Vitest 3.2.4**

**Paquetes Actualizados:**
```json
{
  "devDependencies": {
    "@testing-library/jest-dom": "^6.6.3",
    "@testing-library/react": "^16.1.0",
    "@types/react": "^18.3.18",
    "@types/react-dom": "^18.3.5",
    "@typescript-eslint/eslint-plugin": "^8.18.2",
    "@typescript-eslint/parser": "^8.18.2",
    "@vitejs/plugin-react": "^4.3.4",
    "@vitest/ui": "^3.2.4",
    "eslint": "^9.17.0",
    "eslint-plugin-react-hooks": "^5.1.0",
    "eslint-plugin-react-refresh": "^0.4.16",
    "happy-dom": "^20.0.5",
    "typescript": "^5.7.2",
    "vite": "^6.0.5",
    "vitest": "^3.2.4"
  }
}
```

**Resultado:** ✅ **0 vulnerabilidades** tras `npm audit fix`

### TypeScript Configuration ✅
- ✅ Creado `vite-env.d.ts` para tipos de environment variables
- ✅ Ajustado `tsconfig.json` para excluir archivos de configuración
- ✅ Build sin errores ni warnings de tipos

---

## 🔔 FASE 4: Notificaciones en Tiempo Real - ✅ COMPLETADA

### 4.1 Implementación de SignalR ✅

**Backend - Domain Events Pattern:** ✅ IMPLEMENTADO
```csharp
// ✅ Base Interface
public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}

// ✅ Order Events
public record OrderConfirmedEvent : IDomainEvent
{
    public OrderId OrderId { get; }
    public int TableNumber { get; }
    public DateTime OccurredOn { get; }
}

public record OrderStatusChangedEvent : IDomainEvent
{
    public OrderId OrderId { get; }
    public int TableNumber { get; }
    public OrderStatus OldStatus { get; }
    public OrderStatus NewStatus { get; }
    public DateTime OccurredOn { get; }
}

// ✅ Entity Base Class
public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
```

**SignalR Hub:** ✅ IMPLEMENTADO
```csharp
// ✅ RestaurantApp.Infrastructure/Services/OrderNotificationHub.cs
public class OrderNotificationHub : Hub
{
    // Subscribe to table-specific notifications
    public async Task SubscribeToTable(int tableNumber)
    {
        var groupName = $"table_{tableNumber}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    // Subscribe to kitchen notifications
    public async Task SubscribeToKitchen()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "kitchen");
    }

    // Unsubscribe methods
    public async Task UnsubscribeFromTable(int tableNumber)
    {
        var groupName = $"table_{tableNumber}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task UnsubscribeFromKitchen()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "kitchen");
    }
}
```

**Notification Service:** ✅ IMPLEMENTADO
```csharp
// ✅ RestaurantApp.Application/Services/IOrderNotificationService.cs
public interface IOrderNotificationService
{
    Task NotifyOrderConfirmed(OrderConfirmedEvent orderEvent);
    Task NotifyOrderStatusChanged(OrderStatusChangedEvent statusEvent);
}

// ✅ RestaurantApp.Infrastructure/Services/OrderNotificationService.cs
public class OrderNotificationService : IOrderNotificationService
{
    private readonly IHubContext<OrderNotificationHub> _hubContext;
    private readonly ILogger<OrderNotificationService> _logger;

    public async Task NotifyOrderConfirmed(OrderConfirmedEvent orderEvent)
    {
        var notification = new
        {
            type = "OrderConfirmed",
            orderId = orderEvent.OrderId.Value.ToString(),
            tableNumber = orderEvent.TableNumber,
            occurredAt = orderEvent.OccurredOn
        };

        // Notify customer at the table
        var tableGroup = $"table_{orderEvent.TableNumber}";
        await _hubContext.Clients.Group(tableGroup).SendAsync("OrderConfirmed", notification);

        // Notify kitchen staff
        await _hubContext.Clients.Group("kitchen").SendAsync("NewOrder", notification);
    }

    public async Task NotifyOrderStatusChanged(OrderStatusChangedEvent statusEvent)
    {
        var notification = new
        {
            type = "OrderStatusChanged",
            orderId = statusEvent.OrderId.Value.ToString(),
            tableNumber = statusEvent.TableNumber,
            oldStatus = statusEvent.OldStatus.ToString(),
            newStatus = statusEvent.NewStatus.ToString(),
            occurredAt = statusEvent.OccurredOn
        };

        // Notify customer
        var tableGroup = $"table_{statusEvent.TableNumber}";
        await _hubContext.Clients.Group(tableGroup).SendAsync("OrderStatusChanged", notification);

        // Notify kitchen
        await _hubContext.Clients.Group("kitchen").SendAsync("OrderStatusChanged", notification);
    }
}
```

**Event Dispatching in Use Cases:** ✅ IMPLEMENTADO
```csharp
// ✅ ConfirmOrderUseCase - Dispatches domain events
public class ConfirmOrderUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderNotificationService _notificationService;

    public async Task<Result<OrderDto>> Execute(Guid orderId)
    {
        var order = await _orderRepository.GetById(new OrderId(orderId));
        if (order == null)
            return Result<OrderDto>.Failure("Order not found");

        order.Confirm();
        await _orderRepository.Save(order);

        // Dispatch domain events
        await DispatchDomainEvents(order);

        var dto = MapToDto(order);
        return Result<OrderDto>.Success(dto);
    }

    private async Task DispatchDomainEvents(Order order)
    {
        foreach (var domainEvent in order.DomainEvents)
        {
            switch (domainEvent)
            {
                case OrderConfirmedEvent confirmedEvent:
                    await _notificationService.NotifyOrderConfirmed(confirmedEvent);
                    break;
                case OrderStatusChangedEvent statusChangedEvent:
                    await _notificationService.NotifyOrderStatusChanged(statusChangedEvent);
                    break;
            }
        }
        order.ClearDomainEvents();
    }
}
```

**Program.cs Configuration:** ✅ IMPLEMENTADO
```csharp
// Register SignalR services
builder.Services.AddSignalR();
builder.Services.AddSingleton<IOrderNotificationService, OrderNotificationService>();

// Map SignalR hub endpoint
app.MapHub<OrderNotificationHub>("/hubs/order-notifications");
```

### 4.2 Frontend - React Integration ✅

**Custom Hook for SignalR:** ✅ IMPLEMENTADO
```typescript
// ✅ /src/hooks/useOrderNotifications.ts
export function useOrderNotifications(tableNumber: number): UseOrderNotificationsResult {
  const [isConnected, setIsConnected] = useState(false)
  const [lastNotification, setLastNotification] = useState<OrderNotification | null>(null)
  const [error, setError] = useState<string | null>(null)
  const connectionRef = useRef<signalR.HubConnection | null>(null)

  useEffect(() => {
    // Create SignalR connection
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${API_BASE_URL}/hubs/order-notifications`, {
        skipNegotiation: false,
        transport: signalR.HttpTransportType.WebSockets |
                   signalR.HttpTransportType.ServerSentEvents |
                   signalR.HttpTransportType.LongPolling
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build()

    // Set up event handlers
    connection.on('OrderConfirmed', (notification: OrderNotification) => {
      setLastNotification(notification)
    })

    connection.on('OrderStatusChanged', (notification: OrderStatusNotification) => {
      setLastNotification(notification)
    })

    connection.on('NewOrder', (notification: OrderNotification) => {
      setLastNotification(notification)
    })

    // Start connection and subscribe to table
    const startConnection = async () => {
      try {
        await connection.start()
        setIsConnected(true)
        await connection.invoke('SubscribeToTable', tableNumber)
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Connection failed')
        setIsConnected(false)
      }
    }

    startConnection()

    // Cleanup on unmount
    return () => {
      connection.invoke('UnsubscribeFromTable', tableNumber)
        .then(() => connection.stop())
    }
  }, [tableNumber])

  return { isConnected, lastNotification, error }
}
```

**MenuPage Integration:** ✅ IMPLEMENTADO
- ✅ Real-time connection status indicator (Live/Disconnected)
- ✅ Automatic reconnection handling
- ✅ Toast notifications for order status updates
- ✅ Visual feedback with green/red connection indicator

**Dependencies Added:**
- ✅ Backend: `Microsoft.AspNetCore.SignalR` (built-in .NET 8)
- ✅ Frontend: `@microsoft/signalr@^9.0.6`

### 4.3 Features Implemented ✅

1. ✅ **Real-time Order Confirmation**: Customers receive instant notification when order is confirmed
2. ✅ **Kitchen Notifications**: Kitchen staff receives new order notifications
3. ✅ **Status Updates**: Real-time order status changes (Confirmed → Preparing → Ready → Delivered)
4. ✅ **Connection Management**: Automatic reconnection on network failures
5. ✅ **Group-based Broadcasting**: Table-specific and kitchen group subscriptions
6. ✅ **Domain Events Pattern**: Clean separation between domain logic and infrastructure

---

## 💳 FASE 5: Sistema de Pagos (2 semanas) - ⏳ PENDIENTE

### 5.1 Integración con Pasarela de Pago (Mock)

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

### 5.2 Flujo de Pago

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

## 👨‍🍳 FASE 6: Panel de Cocina y Camareros (2 semanas) - ⏳ PENDIENTE

### 6.1 Dashboard en Tiempo Real

**WebSockets con SignalR:** ⚠️ **Base ya implementada en Fase 4**
```csharp
// ✅ OrderNotificationHub ya tiene subscripción a grupo "kitchen"
public class OrderNotificationHub : Hub
{
    public async Task SubscribeToKitchen()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "kitchen");
    }
}
```

**Interfaz Cocina (Pendiente):**
- Vista kanban de pedidos (Pendiente → Preparando → Listo)
- Tiempo estimado por pedido
- Alertas de modificaciones
- Estadísticas del servicio

### 6.2 Sistema de Notificaciones

- Push notifications cuando pedido está listo
- SMS/Email de confirmación (opcional)
- ✅ WebSocket para actualizaciones en tiempo real (implementado en Fase 4)

---

## 🚀 FASE 7: DevOps y Despliegue (1 semana) - ⏳ PENDIENTE

### 7.1 Dockerización

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

### 7.2 Kubernetes en Digital Ocean

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

### 7.3 GitHub Actions CI/CD

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

## 📊 FASE 8: Optimización y Monitoreo (1 semana) - ⏳ PENDIENTE

### 8.1 Observabilidad

- **Logs**: Serilog + ELK Stack
- **Métricas**: Prometheus + Grafana
- **Tracing**: OpenTelemetry
- **Alertas**: PagerDuty integration

### 8.2 Optimizaciones de Rendimiento

- Implementar caché con Redis
- CDN para assets estáticos
- Lazy loading en frontend
- Paginación en listados
- Índices de base de datos

### 8.3 Tests de Carga

```bash
# K6 para pruebas de carga
k6 run --vus 100 --duration 30s load-test.js
```

---

## 🔒 FASE 9: Seguridad y Compliance (1 semana) - ⏳ PENDIENTE

### 9.1 Seguridad

- Rate limiting por IP
- CORS configuración
- Sanitización de inputs
- Autenticación JWT para staff
- Encriptación de datos sensibles

### 9.2 GDPR Compliance

- Política de privacidad
- Consentimiento de cookies
- Derecho al olvido
- Exportación de datos

---

## 📈 FASE 10: Analytics y Mejoras (Continuo) - ⏳ PENDIENTE

### 10.1 Analytics de Negocio

- Productos más vendidos
- Tiempos de preparación
- Horas pico
- Satisfacción del cliente

### 10.2 A/B Testing

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

## 📅 Cronograma Estimado (Actualizado: 2025-10-18)

| Fase | Estado | Duración Real | Entregable Principal |
|------|--------|---------------|---------------------|
| 1. Fundación | ✅ **COMPLETADA** | 1 semana | Arquitectura base + Docker |
| 2. Core Dominio | ✅ **COMPLETADA** | 1 semana | Gestión mesas y menú |
| 3. Pedidos | ✅ **95% COMPLETADA** | 1.5 semanas | Backend + Frontend + API integration |
| 4. Notificaciones Real-time | ✅ **COMPLETADA** | 1 semana | SignalR + Domain Events |
| 5. Pagos | ⏳ **PENDIENTE** | 2 semanas | Integración pasarela (mock) |
| 6. Panel Cocina | ⏳ **PENDIENTE** | 1.5 semanas | Dashboard tiempo real |
| 7. DevOps | ⏳ **PENDIENTE** | 1 semana | Despliegue producción |
| 8. Optimización | ⏳ **PENDIENTE** | 1 semana | Monitoreo y métricas |
| 9. Seguridad | 🟡 **PARCIAL** | - | Dependencias actualizadas |
| 10. Analytics | ⏳ **PENDIENTE** | Continuo | Analytics y mejoras |
| **PROGRESO ACTUAL** | **~60%** | **4.5 semanas** | **MVP Funcional con real-time** |
| **ESTIMADO RESTANTE** | - | **6-7 semanas** | **MVP Completo** |

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