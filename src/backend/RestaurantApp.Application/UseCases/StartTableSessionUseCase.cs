using RestaurantApp.Application.DTOs;
using RestaurantApp.Application.Ports;
using RestaurantApp.Domain.Exceptions;
using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Application.UseCases;

public class StartTableSessionUseCase
{
    private readonly ITableRepository _tableRepository;

    public StartTableSessionUseCase(ITableRepository tableRepository)
    {
        _tableRepository = tableRepository;
    }

    public async Task<Result<TableSessionDto>> Execute(int tableNumber)
    {
        try
        {
            var tableId = new TableId(tableNumber);
            var table = await _tableRepository.GetById(tableId);

            if (table == null)
            {
                return Result<TableSessionDto>.Failure(
                    $"Table {tableNumber} does not exist.");
            }

            table.StartSession();
            await _tableRepository.Save(table);

            var dto = new TableSessionDto(
                table.ActiveSession!.Id.Value,
                table.Id.Value,
                table.ActiveSession.StartedAt);

            return Result<TableSessionDto>.Success(dto);
        }
        catch (DomainException ex)
        {
            return Result<TableSessionDto>.Failure(ex.Message);
        }
    }
}
