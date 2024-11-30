using Hangfire.Database;
using Hangfire.Models.Entities;
using Hangfire.Repositories.Interface;
using Hangfire.Shared.Helper;
using Microsoft.Extensions.Logging;

namespace Hangfire.Repositories;

public class PdpaRepository : IPdpaRepository
{
    private readonly ILogger<PdpaRepository> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IBaseRepository _baseRepo;

    public PdpaRepository(ILogger<PdpaRepository> logger, ApplicationDbContext context, IBaseRepository baseRepository)
    {
        _logger = logger;
        _context = context;
        _baseRepo = baseRepository;
    }

    public async Task Add(TempDwhPdpa obj)
    {
        try
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            // Check if the entity already exists
            var existingEntity = _context.TempDwhPdpas
                .FirstOrDefault(pdpa => pdpa.AccountSuffix == obj.AccountSuffix);

            if (existingEntity != null)
                await Delete(obj.AccountSuffix);

            // Add the new TempDwhPdpa object
            _context.TempDwhPdpas.Add(obj);

            // Save changes asynchronously
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            // Log the exception with detailed information
            _logger.LogError(ex, $"An error occurred while inserting TempDwhPdpa with AccountNo {obj.AccountSuffix}: {ex.Message}");
            await _baseRepo.BatchExceptionLog("PDPA Error.", $"{MethodHelper.CurrentMethodName()}: {ex.Message}", null);
        }
    }

    public async Task Add(TdrPdpaDelLog obj)
    {
        try
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            // Add the new TempDwhPdpa object
            _context.TdrPdpaDelLogs.Add(obj);

            // Save changes asynchronously
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log the exception with detailed information
            _logger.LogError(ex, $"An error occurred while inserting TdrPdpaDelLog with AccountNo {obj.AccountSuffix}: {ex.Message}");
            await _baseRepo.BatchExceptionLog("PDPA Error.", $"{MethodHelper.CurrentMethodName()}: {ex.Message}", null);
        }
    }

    public string? findCustName(TempDwhPdpa obj)
    {
        return _context.TdrMsts
                .Where(w => w.account_no + w.suffix_no == obj.AccountSuffix)
                .FirstOrDefault()?.cust_name;

    }

    public TempDwhPdpa? Get(TempDwhPdpa obj)
    {
        return _context.TempDwhPdpas
                .FirstOrDefault(pdpa => pdpa.AccountSuffix == obj.AccountSuffix);
    }

    public async Task<List<TempDwhPdpa>> Get(DateTime asOfDate)
    {
        try
        {
            // Query the database to get the list of TempDwhPdpa based on the asOfDate
            var result = _context.TempDwhPdpas
                .Where(pdpa => pdpa.CreateDate.Date == asOfDate.Date)
                .ToList();

            return result;
        }
        catch (Exception ex)
        {
            // Log the exception with detailed information
            _logger.LogError(ex, $"An error occurred while retrieving TempDwhPdpa records for date {asOfDate}: {ex.Message}");
            await _baseRepo.BatchExceptionLog("PDPA Error.", $"{MethodHelper.CurrentMethodName()}: {ex.Message}", null);

            // Optionally, return an empty list or handle the error as needed
            return new List<TempDwhPdpa>();
        }
    }

    public async Task<List<TdrPdpaDelLog>> GetLogPdpa(DateTime asOfDate)
    {
        try
        {
            // Query the database to get the list of TempDwhPdpa based on the asOfDate
            var result = _context.TdrPdpaDelLogs
                        .Where(pdpa => pdpa.DelDate.Date == asOfDate.Date)
                        .ToList();

            return result;
        }
        catch (Exception ex)
        {
            // Log the exception with detailed information
            _logger.LogError(ex, $"An error occurred while retrieving TempDwhPdpa records for date {asOfDate}: {ex.Message}");
            await _baseRepo.BatchExceptionLog("PDPA Error.", $"{MethodHelper.CurrentMethodName()}: {ex.Message}", null);

            // Optionally, return an empty list or handle the error as needed
            return new List<TdrPdpaDelLog>();
        }
    }
    public async Task DeleteAll(List<TempDwhPdpa> list)
    {
        try
        {
            // Remove each entity from the context
            _context.TempDwhPdpas.RemoveRange(list);

            // Save changes asynchronously
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            // Log the exception with detailed information
            _logger.LogError(ex, $"An error occurred while deleting multiple TempDwhPdpa records: {ex.Message}");
            await _baseRepo.BatchExceptionLog("PDPA Error.", $"{MethodHelper.CurrentMethodName()}: {ex.Message}", null);

            // Optionally, rethrow the exception or handle it as needed
            throw new Exception(ex.Message);
        }
    }

    public async Task Delete(string accountSuffix)
    {
        try
        {
            // Use the primary key property for finding the entity
            var existingEntity = _context.TempDwhPdpas
            .FirstOrDefault(e => e.AccountSuffix == accountSuffix);

            if (existingEntity == null)
                throw new InvalidOperationException($"Entity with AccountSuffix {accountSuffix} not found.");

            // Remove the entity
            _context.TempDwhPdpas.Remove(existingEntity);

            // Save changes asynchronously
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            // Log the exception with detailed information
            _logger.LogError(ex, $"An error occurred while deleting TempDwhPdpa with AccountNo {accountSuffix}: {ex.Message}");
            await _baseRepo.BatchExceptionLog("PDPA Error.", $"{MethodHelper.CurrentMethodName()}: {ex.Message}", null);

            // Optionally, rethrow the exception or handle it as needed
            throw new Exception(ex.Message);
        }
    }
}