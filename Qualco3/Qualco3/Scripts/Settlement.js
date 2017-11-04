/*
* Returns the Monthly Payment
*/

function getMonthlyPayment(DownPayment, Interest, Installments)
{
    DownPayment = parseFloat(DownPayment);
    Interest = parseFloat(Interest);
    Installments = parseFloat(Installments);
    
    var _return = ((DownPayment * Math.pow(1 + Interest, Installments)) / ((Math.pow((1 + Interest), Installments) - 1) / ((1 + Interest) - 1)));
    _return = _return.toFixed(2);
    return _return;
}