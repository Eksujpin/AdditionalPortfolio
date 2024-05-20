package dk.itu.moapd.scootersharing.models

import androidx.room.ColumnInfo
import androidx.room.Entity
import androidx.room.PrimaryKey
import java.text.SimpleDateFormat
import java.util.*

@Entity (tableName = "scooter_table")
class Scooter(
    @ColumnInfo(name = "name") var scooterName: String,
    @ColumnInfo(name = "lockStatus") var scooterLocked: lockStatus,
    @ColumnInfo(name = "Battery") var scooterBattery: Int,
    @ColumnInfo(name = "location") var scooterLocation: String,
    @ColumnInfo(name = "lat") var scooterLat: Double,
    @ColumnInfo(name = "long") var scooterLong: Double,
    @ColumnInfo(name = "date") var scooterDate: Long,
    @ColumnInfo(name = "picture") var scooterPicture: String
) {

    @PrimaryKey(autoGenerate = true)
    @ColumnInfo(name = "id")
    var id: Int = 0

    fun prettyDate():String{
        val formatter = SimpleDateFormat("dd-MM-y HH:mm")
        val date = Date(scooterDate)
        return formatter.format(date)
    }

    override fun toString(): String {
        return "Name: $scooterName \nLocation $scooterLocation \nTime: "+ prettyDate()
    }
}

enum class lockStatus {
    Locked, Unlocked
}