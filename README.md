# WeatherAPI

- **Язык/бэкенд:** C# (.NET 10 + ASP.NET Core Web API)
- **БД:** PostgreSQL через EF Core
- **Кэш:** in-memory (MemoryCache) + ключи по городу/дате
- **HTTP клиент:** HttpClient с Polly (retry + circuit breaker + таймаут)
- **Статика:** иконки погоды через `/static/icons/`

---

## Что умеет

- Получает погоду на конкретную дату или на 7 дней вперёд
- Кэширует ответы в памяти (TTL настраивается)
- Пишет статистику каждого запроса в БД
- Отдаёт иконки погоды со своего сервера
- Поднимается одной командой через docker compose

---

## Требования для локального запуска

- .NET 10 SDK
- Docker Desktop

---

## Запуск через Docker
```bash
docker compose up --build
```

API будет доступен на `http://localhost:8080/swagger`

БД и миграции применяются автоматически при старте.

---

## Запуск локально без Docker

Сначала поднять PostgreSQL:
```bash
docker run -d --name weatherapi-postgres \
  -e POSTGRES_DB=weatherapi \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 postgres:16
```

Применить миграции:
```bash
dotnet ef database update \
  --project src/WeatherAPI.Infrastructure \
  --startup-project WeatherAPI.Api
```

Запустить:
```bash
cd WeatherAPI.Api
dotnet run
```

Swagger: `http://localhost:5022/swagger`

---

## Переменные окружения

| Переменная | Описание | Дефолт |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | Строка подключения PostgreSQL | localhost:5432 |
| `BaseUrl` | Базовый URL для генерации ссылок на иконки | http://localhost:5000 |
| `Cache__TtlMinutes` | Время жизни кэша в минутах | 30 |

---

## Примеры запросов

Погода на день:
```
GET /api/weather/London?date=2026-03-30
```

Прогноз на неделю:
```
GET /api/weather/London/week
```

Топ городов по запросам:
```
GET /api/stats/top-cities?from=2026-01-01&to=2026-12-31&limit=10
```

История запросов с пагинацией:
```
GET /api/stats/requests?from=2026-01-01&to=2026-12-31&page=1&pageSize=20
```

Иконка погоды:
```
GET /static/icons/clear.png
```

---

## Тесты
```bash
dotnet test tests/WeatherAPI.Tests
```

9 тестов: маппинг погодных кодов и логика кэша (хит/мисс).

---

## Структура проекта
```
src/
  WeatherAPI.Domain/        - модели, энамы
  WeatherAPI.Application/   - интерфейсы, сервисы, DTO
  WeatherAPI.Infrastructure/ - EF Core, HTTP клиенты, маппинг
WeatherAPI.Api/             - контроллеры, middleware, Program.cs
tests/
  WeatherAPI.Tests/         - юнит тесты
```

Внешний источник данных - [Open-Meteo](https://open-meteo.com/), без API ключа.

Скрины со Swagger:
<img width="1917" height="1000" alt="swag1" src="https://github.com/user-attachments/assets/ab5f5a29-f5f1-472d-910e-8d32fbc6d05c" />
<img width="1909" height="978" alt="swag2" src="https://github.com/user-attachments/assets/eb7056c3-b13b-411d-ab2b-420e75ff0cd8" />
<img width="1919" height="988" alt="swag3" src="https://github.com/user-attachments/assets/adf0ab31-cce1-44d1-93d0-88a133334f3d" />
<img width="1919" height="1001" alt="swag4" src="https://github.com/user-attachments/assets/fa03e63a-af8e-4384-a4d5-4cd35beba853" />

