using Igor_AIS_Proj.Business.Interfaces;
using Igor_AIS_Proj.Models;


namespace Igor_AIS_Proj.Business
{
    public class MovementBusiness :  IMovementBusiness
    {
        private IMovementPersistence _movementPersistence;
        public MovementBusiness(IMovementPersistence movementPersistence) => _movementPersistence = movementPersistence;


        public async Task<Movement> GetById(int id1) => await _movementPersistence.GetById(id1);
        public async Task<bool> Delete(int id1) => await _movementPersistence.Delete(id1);

        public List<Movement> GetAll() => _movementPersistence.GetAll();

        public async Task<Movement> Create(Movement movement) => await _movementPersistence.Create(movement);
        public async Task<bool> Update(Movement movement) => await _movementPersistence.Update(movement);

    }
}
