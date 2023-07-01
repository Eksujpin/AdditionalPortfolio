package dk.itu.moapd.scootersharing.models

import androidx.room.ColumnInfo
import androidx.room.Entity
import androidx.room.PrimaryKey
import java.text.SimpleDateFormat
import java.util.*
import kotlin.math.*

@Entity(tableName = "ride_table")
class Ride(
    @ColumnInfo(name = "status") var rideStatus: runStatus,
    @ColumnInfo(name = "scooter") var rideScooterID: Int,
    @ColumnInfo(name = "rideUser") var rideUser: String,
    @ColumnInfo(name = "startLat") var startLat: Double,
    @ColumnInfo(name = "startLong") var startLong: Double,
    @ColumnInfo(name = "startTime") var rideStartTime: Long,
    @ColumnInfo(name = "endLat") var endLat: Double?,
    @ColumnInfo(name = "endLong") var endLong: Double?,
    @ColumnInfo(name = "endTime") var rideEndTime: Long?,
    @ColumnInfo(name = "price") var ridePrice: Double?
) {

    @PrimaryKey(autoGenerate = true)
    @ColumnInfo(name = "id")
    var id: Int = 0

    fun rideTime(): String{
        val endDate = Date(rideEndTime!!)
        val startDate = Date(rideStartTime)
        var different: Long = endDate.time - startDate.time

        val secondsInMilli: Long = 1000
        val minutesInMilli = secondsInMilli * 60
        val hoursInMilli = minutesInMilli * 60

        //long elapsedDays = different / daysInMilli;
        //different = different % daysInMilli;

        val elapsedHours = different / hoursInMilli
        different %= hoursInMilli

        val elapsedMinutes = different / minutesInMilli
        different %= minutesInMilli

        val elapsedSeconds = different / secondsInMilli

        return " H: $elapsedHours M: $elapsedMinutes S: $elapsedSeconds"
    }

    //math found on the web to calculate crowsline from one lat*lon to another in meters
    fun distanceMeters():Double{
        val r = 6371e3 // metres
        val l1 = startLat * Math.PI/180 // φ, λ in radians
        val l2 = endLat!! * Math.PI/180
        val cal01 = (endLat!! -startLat) * Math.PI/180
        val cal02 = (endLong!!-startLong) * Math.PI/180
        val a = sin(cal01/2) * sin(cal01/2) +
                cos(l1) * cos(l2) *
                sin(cal02/2) * sin(cal02/2)
        val c = 2 * atan2(sqrt(a), sqrt(1-a))
        val d = r * c // in metres
        return round(d * 10) / 10
    }

    fun prettyDateStart():String{
        val formatter = SimpleDateFormat("dd-MM-y HH:mm")
        val date = Date(rideStartTime)
        return formatter.format(date)
    }

    fun prettyDateEnd():String{
        val formatter = SimpleDateFormat("dd-MM-y HH:mm")
        val date = Date(rideEndTime!!)
        return formatter.format(date)
    }


}

enum class runStatus {
    Cancelled, Reserved, Running, Finished
}