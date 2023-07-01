package dk.itu.moapd.scootersharing.models

import androidx.lifecycle.LiveData
import androidx.room.*
@Dao
interface ScooterDAO {

    @Insert
    suspend fun insert(scooter: Scooter)

    @Update
    suspend fun update (scooter: Scooter)

    @Delete
    suspend fun delete (scooter: Scooter)

    @Query("DELETE FROM scooter_table")
    suspend fun deleteAll()

    @Query("SELECT * FROM scooter_table ORDER BY ID ASC") //ORDER BY ID ASC
    fun getOrdedScooters(): LiveData<List<Scooter>>

    @Query("SELECT * FROM scooter_table WHERE name LIKE :name LIMIT 1")
    fun getByName(name: String): LiveData<Scooter?>

    @Query("SELECT * FROM scooter_table WHERE id LIKE :id ")
    fun getById(id: Int): LiveData<Scooter>

}