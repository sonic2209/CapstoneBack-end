using ESMS.Data.EF;
using ESMS.Data.Entities;
using ESMS.Utilities.Exceptions;
using ESMS.ViewModels.Services.Position;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESMS.Application.Services.Positions
{
    public class PositionService : IPositionService
    {
        private readonly ESMSDbContext _context;

        public PositionService(ESMSDbContext context)
        {
            _context = context;
        }

        public async Task<int> Create(PositionCreateRequest request)
        {
            var position = new Position()
            {
                Name = request.Name,
                Description = request.Description,
            };
            _context.Positions.Add(position);
            await _context.SaveChangesAsync();
            return position.PosID;
        }

        public async Task<int> Delete(int positionID)
        {
            var position = await _context.Positions.FindAsync(positionID);
            if (position == null) throw new ESMSException($"Cannot find a projectID: {positionID}");

            _context.Positions.Remove(position);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> Update(PositionUpdateRequest request)
        {
            var position = await _context.Positions.FindAsync(request.PosID);
            if (position == null) throw new ESMSException($"Cannot find a projectID: {request.PosID}");

            position.Name = request.Name;
            position.Description = request.Description;

            return await _context.SaveChangesAsync();
        }
    }
}