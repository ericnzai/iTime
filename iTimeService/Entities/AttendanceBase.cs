using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iTimeService.Concrete;
using System.Data.Entity;

namespace iTimeService.Entities
{
    public class AttendanceBase
    {
        public AttendanceBase()
        {
            adjlost = 0;
            finallost = 0;
            finalot = 0;
            lunchlost = 0;
            latein = 0;
            latego = 0;
            earlygo = 0;
            earlyin = 0;
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int id { get; set; }
        public int empid { get; set; }
        public virtual EnrolledEmployee EnrolledEmployee { get; set; }
        public int deptid { get; set; }
        public int shiftid { get; set; }
        public virtual ShiftType Shift { get; set; }

        public int daytype { get; set; }
        public string shifttype { get; set; }
        public DateTime attenddt { get; set; }
        public bool isabsent { get; set; }
        public int attstatus { get; set; }
        public DateTime? timein { get; set; }
        public DateTime? timeout { get; set; }
        public DateTime? timeinE { get; set; }
        public DateTime? timeoutE { get; set; }
        public DateTime? breakout { get; set; }
        public DateTime? breakin { get; set; }
        public decimal totalhrscount { get; set; }
        public decimal totalhrsworked { get; set; }
        public decimal normalhrsworked { get; set; }
        public decimal otHD { get; set; }
        public decimal otND { get; set; }
        public decimal breaktm { get; set; }
        public decimal leaveouts { get; set; }
        public decimal losthrs { get; set; }
        public decimal adjot { get; set; }
        public bool timeinM { get; set; }
        public bool timeoutM { get; set; }
        public bool breakoutM { get; set; }
        public bool breakinM { get; set; }
        public int weekday { get; set; }
        public string comment { get; set; }
        public decimal earlygo { get; set; }
        public decimal latein { get; set; }
        public decimal timeinP { get; set; }
        public decimal timeoutP { get; set; }
        public decimal lunchP { get; set; }

        public decimal finallost { get; set; }
        public decimal adjlost { get; set; }
        public decimal finalot { get; set; }
        public decimal lunchlost { get; set; }
        public bool chlocked { get; set; }

        [NotMapped]
        public bool isNewRecord { get; set; }
        [NotMapped]
        public bool extendNxtDay { get; set; }
        [NotMapped]
        public TimeSpan allowedbreak { get; set; }
        [NotMapped]
        public decimal maxhrsAllowed { get; set; }
        [NotMapped]
        public decimal stdhrsAllowed { get; set; }
        [NotMapped]
        public decimal maxOTHrs { get; set; }
        [NotMapped]
        public bool isHol { get; set; }
        [NotMapped]
        public DateTime shiftGraceAlowed { get; set; }
        [NotMapped]
        public DateTime shiftInTime { get; set; }
        [NotMapped]
        public DateTime shiftOutTime { get; set; }
        [NotMapped]
        public bool isDynamic { get; set; }
        [NotMapped]
        public decimal earlyin { get; set; }
        [NotMapped]
        public decimal latego { get; set; }
        [NotMapped]
        public enBreaks breakType { get; set; }
        public int compid { get; set; }
        public int userid { get; set; }

        
        public void calcOTSeparate()
        {
            try
            {
                otND = this.totalhrsworked - stdhrsAllowed;

                if (otND > maxOTHrs) otND = maxOTHrs;
                else if (otND < Common.Common._minOTHrs) otND = 0;
                finalot = adjot + otND;
                otHD = 0;
                if (this.EnrolledEmployee != null)
                {
                    if (!this.EnrolledEmployee.iscalcot)
                    {
                        otND = 0;
                        finalot = 0;
                        adjot = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Common.Common._processedOk = false;
                Common.Common._exception = ex;
            }
        }

        public void calcLostSeparate()
        {
            try
            {
                adjlost = 0;
                losthrs = latein + earlygo + lunchlost;
                finallost = losthrs + adjlost;

                if (this.EnrolledEmployee != null)
                {
                    if (!this.EnrolledEmployee.islosthrs)
                    {
                        losthrs = 0;
                        adjlost = 0;
                        finallost = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Common.Common._processedOk = false;
                Common.Common._exception = ex;
            }
        }

        public void calcTTHrsCount()
        {
            try
            {
               
                TimeSpan? ttHrsCount;
             if (this.timein.HasValue && this.timeout.HasValue)
                {
                    ttHrsCount = timeout - timein;

                    decimal totalHrsCnt = Convert.ToDecimal(TimeSpan.Parse(ttHrsCount.ToString()).TotalHours);

                    if (totalHrsCnt > maxhrsAllowed)
                    {
                        totalhrscount = maxhrsAllowed;
                    }
                    else totalhrscount = Math.Round(totalHrsCnt, 2);

                }
                else totalhrscount = 0;

                if (this.totalhrscount == 0)
                    isabsent = true;
                else isabsent = false;
            }
            catch (Exception ex)
            {
                Common.Common._processedOk = false;
                Common.Common._exception = ex;
            }
        }

        public void calcTTHrsWorked()
        {
            try
            {
                if (totalhrscount < 0) totalhrscount = 0;
                if (breaktm < 0) breaktm = 0;
                if (leaveouts < 0) leaveouts = 0;
                //check if modified by client and ignore breaks and set totalhrs count
                if ((timeinM == true || timeoutM == true) && totalhrsworked > 0 )
                    totalhrscount = totalhrsworked;
                else   totalhrsworked = totalhrscount - ((breaktm + leaveouts) / 60);
                //------------------end check--------------------------------------------
                if (totalhrsworked < 0)
                    totalhrsworked = 0;
                else if (totalhrsworked < Common.Common._minWorkHrs)
                {
                    totalhrsworked = 0;
                    isabsent = true;
                }
                else if (totalhrsworked > maxhrsAllowed)
                    totalhrsworked = maxhrsAllowed;
            }
            catch (Exception ex)
            {
                Common.Common._processedOk = false;
                Common.Common._exception = ex;
            }
        }

        public void calcNormalHrsWorked()
        {
            try
            {
                if (this.totalhrsworked > 0 && this.stdhrsAllowed > 0)
                {
                    if (totalhrsworked > stdhrsAllowed)
                    {
                        normalhrsworked = stdhrsAllowed;
                    }
                    else
                    {
                        normalhrsworked = totalhrsworked;
                    }
                }
                else
                {
                    normalhrsworked = 0;
                }
            }
            catch (Exception ex)
            {
                Common.Common._processedOk = false;
                Common.Common._exception = ex;
            }
        }

        public void calcTTBreaks()
        {
            try
            {
                if (this.breakType == enBreaks.Dynamic)
                {
                    if (this.breakout.HasValue && this.breakin.HasValue)
                    {
                        DateTime bIn = DateTime.Parse(breakin.ToString());
                        DateTime bOut = DateTime.Parse(breakout.ToString());
                        TimeSpan ttBrks = (bIn - bOut);
                        this.breaktm = Convert.ToDecimal(ttBrks.TotalMinutes);

                        if (this.breaktm < Common.Common._lunchMinDuration) this.breaktm = Common.Common._lunchMinDuration;
                        //if (ttBrks > allowedbreak) this.breaktm = Decimal.Parse(ttBrks.ToString());
                        //else this.breaktm = Decimal.Parse(this.allowedbreak.TotalHours.ToString());
                    }
                    else
                    {
                        if (isHol) //do not deduct min duration on holiday if user has not gone for break
                        {
                            breaktm = 0;
                        }
                        else
                            breaktm = Common.Common._lunchMinDuration;
                    }
                    //else this.breaktm = Decimal.Parse(this.allowedbreak.TotalHours.ToString());
                }
                else
                {
                    this.breaktm = Common.Common._lunchMinDuration;
                }
            }
            catch (Exception ex)
            {
                Common.Common._processedOk = false;
                Common.Common._exception = ex;
            }
        }

        public void calcOT()
        {
            try
            {
                decimal otMins = otND * 60;
                if (losthrs > otMins)
                {
                    losthrs = losthrs - otMins;
                    otND = 0;
                }
                else
                {
                    otMins = otMins - losthrs;
                    losthrs = 0;
                    otND = Math.Round(otMins / 60, 2);
                }

                //otND = Math.Floor(otND);
                losthrs = Math.Floor(losthrs);
                finalot = adjot + otND;
                finallost = adjlost + losthrs;
            }
            catch (Exception ex)
            {
                Common.Common._exception = ex;
            }
        }

        public void setDayType()
        {
            try
            {
               if(isHol) this.daytype = (int)enDayType.Holiday;
               else this.daytype = (int)enDayType.Normal;
            }
            catch (Exception ex)
            {
                Common.Common._processedOk = false;
                Common.Common._exception = ex;
            }
        }

        public void calcLateIn() 
        {
            try
            {
                DateTime shiftGraceAlowed = Common.Common._shiftGraceIn;
                DateTime shiftInTime = Common.Common._shiftIn;
                TimeSpan lostMins;
                if (this.shifttype == "DAY")
                {
                    shiftGraceAlowed = Common.Common._shiftGraceInD;
                    shiftInTime = Common.Common._shiftInD;

                }
                else if (this.shifttype == "NIGHT")
                {
                    shiftGraceAlowed = Common.Common._shiftGraceInN;
                    shiftInTime = Common.Common._shiftInN;
                }
                if (this.timein > shiftGraceAlowed)
                {
                    lostMins = TimeSpan.Parse((timein - shiftInTime).ToString());
                    latein = Convert.ToDecimal(lostMins.TotalMinutes);
                }
                else
                {
                    latein = 0;
                }

                if (latein > 0 && latein < Common.Common._shiftInPenalty) latein = Common.Common._shiftInPenalty;
                latein = Math.Floor(latein);
            }
            catch (Exception ex)
            {
                Common.Common._processedOk = false;
                Common.Common._exception = ex;
            }
        }
        public void calcEarlyGo()
        {
            try
            {
                DateTime shiftGraceAlowed = Common.Common._shiftGraceOut;
                DateTime shiftOutTime = Common.Common._shiftOut;
                TimeSpan lostMins;

                if (this.shifttype == "DAY")
                {
                    shiftGraceAlowed = Common.Common._shiftGraceOutD;
                    shiftOutTime = Common.Common._shiftOutD;
                }
                else if (this.shifttype == "NIGHT")
                {
                    shiftGraceAlowed = Common.Common._shiftGraceOutN;
                    shiftOutTime = Common.Common._shiftOutN;
                }
                if (this.timeout < shiftGraceAlowed)
                {
                    lostMins = TimeSpan.Parse((shiftOutTime - timeout).ToString());
                    if (totalhrsworked >= stdhrsAllowed)
                    {
                        earlygo = 0;
                    }
                    else earlygo = Convert.ToDecimal(lostMins.TotalMinutes);
                }
                else
                {
                    earlygo = 0;
                }
                
                if (earlygo > 0 && earlygo < Common.Common._shiftOutPenalty) earlygo = Common.Common._shiftOutPenalty;
                //check if modified by client and recalculate early go based on stdwork hrs
                if (timeoutM == true && totalhrsworked < stdhrsAllowed) earlygo = Convert.ToDecimal((stdhrsAllowed - totalhrsworked) * 60);
               
                earlygo = Math.Floor(earlygo);
            }
            catch (Exception ex)
            {
                Common.Common._processedOk = false;
                Common.Common._exception = ex;
            }
        }
        public void calcLunchLost()
        {
            try
            {
                if (this.breakType == enBreaks.Fixed || isHol == true)
                {
                    lunchlost = 0; lunchP = 0;
                }
                else
                {
                    decimal lpenalty = Common.Common._lunchPenalty;
                    decimal lunchDuration = Convert.ToDecimal(allowedbreak.TotalMinutes.ToString());
                    if (Common.Common._lunchMinDuration > lunchDuration)
                        lunchDuration = Common.Common._lunchMinDuration;
                    decimal lunchWithPenalty = lunchDuration + lpenalty;

                    //-- Calculating lunch break based on penalty --//
                    if (breaktm > lunchDuration && breaktm < lunchWithPenalty)
                    {
                        lunchP = lpenalty;
                        breaktm = lunchDuration;
                        lunchlost = lunchP;
                    }
                    else if (breaktm > lunchDuration && breaktm > lunchWithPenalty)
                    {
                        lunchP = 0;
                        lunchlost = breaktm - lunchDuration;
                        breaktm = lunchDuration;
                    }

                    if (lunchlost < 0) lunchlost = 0;
                    lunchlost = Math.Floor(lunchlost);

                    if (Convert.ToDecimal(allowedbreak.TotalMinutes.ToString()) <= 0)
                    {
                        lunchlost = 0; lunchP = 0; breaktm = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Common.Common._processedOk = false;
                Common.Common._exception = ex;
            }
        }
        public void adjustOnHoliday() {
            try
            {
                TimeSpan otHDNight;
                if (Common.Common._otDef == enOTDef.OneDayOnly || (Common.Common._otDef == enOTDef.PreNextDay && this.shifttype == "DAY"))
                {
                    if (isHol)
                    {
                        otHD = totalhrsworked;
                        normalhrsworked = 0;
                        otND = 0;
                        losthrs = 0;
                        latein = 0;
                        earlygo = 0;
                        earlyin = 0;
                        lunchlost = 0;
                        latego = 0;
                        finallost = 0;
                        finalot = 0;
                    }
                }
                else if (Common.Common._otDef == enOTDef.PreNextDay)
                {
                    if (this.shifttype == "NIGHT")
                    {
                        if (isHol)
                        {
                            latein = 0;
                            earlygo = 0;
                            losthrs = 0;
                            normalhrsworked = 0;
                            finalot = 0;
                            finallost = 0;
                            DateTime tIn = DateTime.Parse(timein.ToString());
                            otHDNight = Common.Common._dayEnd - tIn;
                            otHD = Convert.ToDecimal(otHDNight.TotalHours);


                            if (otHD > totalhrsworked)
                            {
                                otHD = totalhrsworked;
                                otND = 0;
                            }
                            else if (otHD < totalhrsworked)
                            {
                                otND = totalhrsworked - otHD;
                            }


                            if (Common.Common._isNextDayHol)
                            {
                                otHD += otND;
                                otND = 0;
                            }
                        }
                        else if (Common.Common._isNextDayHol)
                        {
                            //normalhrsworked = Convert.ToDecimal(Common.Common._dayEnd - DateTime.Parse(timein.ToString()));
                            normalhrsworked = totalhrsworked - stdhrsAllowed;
                            if (normalhrsworked < 0)
                            {
                                otHD = 0;
                                losthrs = -(normalhrsworked);
                                normalhrsworked = totalhrsworked;
                            }
                            else if (normalhrsworked > 0)
                            {
                                losthrs = latein;
                                otHD = normalhrsworked;
                                normalhrsworked = stdhrsAllowed;
                            }
                            otND = 0;
                            if (finalot > 0) finalot = 0;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Common.Common._processedOk = false;
                Common.Common._exception = ex;
            }
        }
        public void setComment()
        {
            try
            {
                if (isabsent)
                {
                    switch (this.attstatus)
                    {
                        case (int)enAttStatus.Leave:
                            this.comment = "LEAVE";
                            break;
                        case (int)enAttStatus.Sick:
                            this.comment = "SICK";
                            break;
                        case (int)enAttStatus.Off:
                            this.comment = "OFF";
                            break;
                        case (int)enAttStatus.Off2:
                            this.comment = "OFF/2";
                            break;
                        case (int)enAttStatus.Sick2:
                            this.comment = "SICK/2";
                            break;
                        default:
                            this.comment = "ABSENT";
                            break;
                    }
                }
                else
                {
                    switch (this.attstatus)
                    {
                        case (int)enAttStatus.Leave:
                            this.comment = "PR LEAVE";
                            break;
                        case (int)enAttStatus.Sick:
                            this.comment = "PR SICK";
                            break;
                        case (int)enAttStatus.Off:
                            this.comment = "PR OFF";
                            break;
                        case (int)enAttStatus.Off2:
                            this.comment = "PR OFF/2";
                            break;
                        case (int)enAttStatus.Sick2:
                            this.comment = "PR SICK/2";
                            break;
                        default:
                            this.comment = "PRESENT";
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Common.Common._processedOk = false;
                Common.Common._exception = ex;
            }
        }
    }
}
