using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Particles
{
    class Particle
    {
        public Vector position,speed;
        public Vector Rspeed,RspeedPrev;
        public double 
            mass,
            radius ,
            k,
            temperature=10,
            time=0;

        double 
            speed_cond = 0.01, 
            temperature_cond = 0.1;



        public delegate void func(List<Particle> particles) ;
        public func f;
        
        static double eps = 0.001,G=1000;
        public Particle(Vector pos,Vector s,
            double m,double r,double koef,double t)
        {
            Rspeed = new Vector(0, 0);
            position = pos;
            mass = m;
            radius = r;
            speed = s;
            k = koef;
            temperature = t;
            f = LennardGons;
        }


 
        public void LennardGons(List<Particle> particles)
        {
            Vector F;
            double
                e=0.01,//??
                sigma=1.51,
                rs = 1.24*sigma,
                rc = 2.73*sigma,  
                k1 = (387072/61009)*(e*Math.Pow(rs,-3)),
                k2 = (24192 / 3211) * (e * Math.Pow(rs, -2));
            foreach (var item in particles)
            {
                F = (item.position - position);
                double r = (position - item.position).Length;
                if (r > eps)
                {
                    RspeedPrev = Rspeed;
                    if (r <= rs)
                    {
                        F *= 4 * e *
                            (Math.Pow(sigma / r, 12)
                            - Math.Pow(sigma / r, 6));
                    }
                    else if (rs < r && r <= rc)
                    {
                        F *= k1 * Math.Pow(r - rc, 3)
                            + k2 * Math.Pow(r - rc, 2); 
                    }
                    else
                    {
                        F *= 0;
                    }
                    if ((Rspeed - RspeedPrev).Length > 0.002)
                    {
                        F *= 0.1;
                    }
                    Rspeed += F;
                   
                }
            }
        }

        public void closeF(List<Particle> particles)
        {
            Vector F;
            foreach (var item in particles)
            {
                double distance = (position - item.position).Length;
                if (distance > eps)
                {
                    F = (item.position - position);
                    F.Normalize();
                    F *= G * Math.Pow(distance, -2) * item.mass / mass;//Гравитация
                    if (distance < 2 * radius)
                    {
                        RspeedPrev = Rspeed;
                        if (distance < radius)
                        {
                            //Отталкивание
                            F = -F * 4 * temperature;
                        }
                        Rspeed += F;

                        if ((RspeedPrev - Rspeed).Length > 0.2)
                        {
                            Rspeed *= 0.9;
                        }

                        if (distance < radius * 1.1)//обмениваемся скоростяси и температурой
                        {
                            F.Normalize();
                            speed += F * item.speed.Length / mass * speed_cond;

                            temperature += (item.temperature - temperature) * temperature_cond;
                        }
                    }
                    else
                    {
                        speed +=  F * Math.Pow(distance, -1) * item.mass / mass;//туть блок для гравитации
                    }
                }
            }
        }
    

        public void change_position()
        {
            position += speed + Rspeed;
            if (temperature>1)
            {
                temperature -= 0.0001*temperature;
            }
            //Rspeed *= 0.95;
        }

    }
}
