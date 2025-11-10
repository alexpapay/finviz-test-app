import { TextField } from "@mui/material";

interface InputProps {
    value: string;
    onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
    placeholder?: string;
    label?: string;
}

export const Input = ({ value, onChange, placeholder, label }: InputProps) => (
    <TextField
        size="small"
        fullWidth
        variant="outlined"
        value={value}
        label={label}
        placeholder={placeholder}
        onChange={onChange}
    />
);
