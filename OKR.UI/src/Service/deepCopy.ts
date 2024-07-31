export function deepCopy(obj: any) {
  if (obj === null || typeof obj !== 'object') {
    return obj;
  }

  if (obj instanceof Date) {
    return new Date(obj);
  }

  if (Array.isArray(obj)) {
    const arrCopy: any[] = [];
    obj.forEach((item, index) => {
      arrCopy[index] = deepCopy(item);
    });
    return arrCopy;
  }

  const objCopy: { [key: string]: any } = {};
  Object.keys(obj).forEach(key => {
    objCopy[key] = deepCopy(obj[key]);
  });
  return objCopy;
}
  