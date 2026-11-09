/** วันที่ Backend ใช้แทนค่าว่าง */
export const PLACEHOLDER_ISO_DATE = '2000-01-01'

/** ตรวจสอบว่า ISO Date เป็นค่าว่างหรือ Placeholder */
export function isEmptyIsoDate(value?: string | null): boolean {
  return !value || value.startsWith(PLACEHOLDER_ISO_DATE)
}

/** แปลง ISO Date String เป็น Date (Local Time) */
export function isoDateStringToDate(value?: string | null): Date | null {
  if (isEmptyIsoDate(value)) return null

  const [year, month, day] = value!.substring(0, 10).split('-').map(Number)

  if (!year || !month || !day) return null

  const date = new Date(year, month - 1, day)

  // ป้องกันวันที่ผิด เช่น 2026-02-31
  if (date.getFullYear() !== year || date.getMonth() !== month - 1 || date.getDate() !== day) {
    return null
  }

  return date
}

/** แปลง Date เป็น ISO Date String (yyyy-MM-dd) */
export function dateToIsoDateString(date: Date): string {
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')

  return `${year}-${month}-${day}`
}

/** แปลง ISO Date String เป็น Display Date (dd/MM/yyyy) */
export function isoDateStringToDisplay(value?: string | null): string {
  const date = isoDateStringToDate(value)

  if (!date) return '-'

  const day = String(date.getDate()).padStart(2, '0')
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const year = date.getFullYear()

  return `${day}/${month}/${year}`
}

/** แปลง Date เป็น Display Date (dd/MM/yyyy) */
export function dateToDisplay(date: Date): string {
  const day = String(date.getDate()).padStart(2, '0')
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const year = date.getFullYear()

  return `${day}/${month}/${year}`
}

/** เปรียบเทียบ Date เฉพาะวัน (ไม่สนเวลา) */
export function isSameOrAfterDate(first: Date, second: Date): boolean {
  return dateToIsoDateString(first) >= dateToIsoDateString(second)
}
